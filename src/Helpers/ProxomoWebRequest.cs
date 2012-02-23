using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace Proxomo
{
    internal class ProxomoWebRequest<t> : IDisposable
    {
        internal delegate void ProxomoCallbackItem(ItemCompletedEventArgs<t> e);
        internal delegate void ProxomoGenericDelegate(ItemCompletedEventArgs<t> e);

        public string token = string.Empty;
        private bool _validateSSLCert = false;
        private CommunicationType _Format = CommunicationType.JSON;
        public CommunicationType Format
        {
            get
            {
                return _Format;
            }
            set
            {
                _Format = value;
            }
        }

        public ProxomoWebRequest(bool validateSSLCert, CommunicationType format)
        {
            _validateSSLCert = validateSSLCert;
            this.Format = format;
        }

        public ProxomoWebRequest(string authToken, bool validateSSLCert, CommunicationType format)
        {
            token = authToken;
            _validateSSLCert = validateSSLCert;
            this.Format = format;
        }

        internal void GetDataItem(string url, string method, string contentType, string content, ProxomoUserCallbackDelegate<t> userCallback)
        {
            ContinuationTokens cTokens = new ContinuationTokens("", "");

            GetDataItem(url, method, contentType, content, userCallback, ref cTokens);

        }

        internal void GetDataItem(string url, string method, string contentType, string content, ProxomoUserCallbackDelegate<t> userCallback, ref ContinuationTokens cTokens)
        {
            RequestStateItem<t> state = new RequestStateItem<t>();

            state.UserCallback = userCallback;
            state.Url = url;
            state.Method = method;
            state.ContentType = contentType;
            state.Request = GetRequest(url, method, ref cTokens);

            if (content.Length > 0)
            {
                state.Content = content;
                state.Request.ContentType = contentType;
                state.Request.BeginGetRequestStream(GetResponseStreamItem_Callback, state);
            }
            else
            {
                state.Request.BeginGetResponse(GetResponseItem_Callback, state);
            }
        }

        internal HttpWebRequest GetRequest(string url, string method, ref ContinuationTokens cTokens)
        {
            string NextPartitionKeyDescriptor = "NextPartitionKey";
            string NextRowKeyDescriptor = "NextRowKey";


            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(url);
            client.Method = method;

            if (!(string.IsNullOrEmpty(token)))
            {
                client.Headers["Authorization"] = token;
            }

            // Include continuation tokens in the request header to the service if they were specified but the caller (i.e. sent in)...
            if (!(string.IsNullOrEmpty(cTokens.NextPartitionKey)) && !(string.IsNullOrEmpty(cTokens.NextPartitionKey)))
            {
                client.Headers[NextPartitionKeyDescriptor] = cTokens.NextPartitionKey;
                client.Headers[NextRowKeyDescriptor] = cTokens.NextRowKey;
            }

            //Windows Phone 7 SDK applications do not have direct access to the socket layer and thus cannot intercept the SSL certificate validation.
            //if (!_validateSSLCert)
            //{
            //    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(returnTrue);
            //}

            return client;
        }

        private void GetResponseStreamItem_Callback(IAsyncResult asyncResult)
        {
            RequestStateItem<t> state = (RequestStateItem<t>)asyncResult.AsyncState;

            try
            {
                Stream postStream = state.Request.EndGetRequestStream(asyncResult);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(state.Content);

                postStream.Write(byteArray, 0, state.Content.Length);
                postStream.Close();

                byteArray = null;

                state.Request.BeginGetResponse(GetResponseItem_Callback, state);
            }
            catch (Exception ex)
            {
                state.UserCallback(new ItemCompletedEventArgs<t> { IsError = true, Error = ex, Result = default(t) });
            }
        }

        private void GetResponseItem_Callback(IAsyncResult asyncResult)
        {
            // Note: we need to instantiate these two objects outside the try statement so that they are available to the catch statements
            // to return useful error information to the caller when an exception is thrown.
            RequestStateItem<t> state = (RequestStateItem<t>)asyncResult.AsyncState;
            ItemCompletedEventArgs<t> args = new ItemCompletedEventArgs<t> { IsError = false, HttpRespCode = 0, HttpRespMessage = "", Error = null, UserData = null, Result = default(t) };

            try
            {

                state.Response = (HttpWebResponse)state.Request.EndGetResponse(asyncResult);

                // Return back to caller the continuation tokens returned by the service response (if any) ... 
                string NextPartitionKeyDescriptor = "NextPartitionKey";
                string NextRowKeyDescriptor = "NextRowKey";

                ContinuationTokens cTokensIfAny = new ContinuationTokens("", "");

                cTokensIfAny.NextPartitionKey = state.Response.Headers[NextPartitionKeyDescriptor];
                cTokensIfAny.NextRowKey = state.Response.Headers[NextRowKeyDescriptor];

                using (Stream resultStream = state.Response.GetResponseStream()) // See note in the WebException catch below regarding exceptions that could be thrown by this line...                
                {
                    using (StreamReader sreader = new StreamReader(resultStream))
                    {
                        state.Request = null;

                        args.Error = null;
                        args.cTokens = cTokensIfAny;

                        if (this.Format == CommunicationType.XML)
                        {
                            args.Result = ReturnXML(sreader);

                            if (!(state.CallBack == null))
                            {
                                // The only case were we set this Callback instead of the UserCallback is during the Init of the SDK...
                                // The reason is that for now since we do not want the Init call to call back into a delegate in the user side, we are just calling back
                                // into an 'internal' delegate were we can update the AuthToken info before returning to user in a "synchronous" fashion
                                state.CallBack(args);
                            }
                            else
                            {
                                // All other entry points call back into a delegate sent in by the user.  Thus they bahave asynchronously.
                                state.UserCallback(args);
                            }
  
                        }
                        else if (this.Format == CommunicationType.JSON)
                        {
                            args.Result = ReturnJSON(sreader);

                            if (!(state.CallBack == null))
                            {
                                // The only case were we set this Callback instead of the UserCallback is during the Init of the SDK...
                                // The reason is that for now since we do not want the Init call to call back into a delegate in the user side, we are just calling back
                                // into an 'internal' delegate were we can update the AuthToken info before returning to user in a "synchronous" fashion
                                state.CallBack(args);
                            }
                            else
                            {
                                // All other entry points call back into a delegate sent in by the user.  Thus they bahave asynchronously.
                                state.UserCallback(args);
                            }
                        }
                        else
                        {
                            state.CallBack(new ItemCompletedEventArgs<t> { IsError = true, Error = null, Result = default(t) });
                        }
                        sreader.Close();  //Fix for Windows Phone network bug
                    }
                    resultStream.Close();  //Fix for Windows Phone network bug
                }
                state.Response.Close();  //Fix for Windows Phone network bug
                state.Response = null;
                state = null;
            }
            //catch (NullReferenceException nullex)
            //{
            //    state.CallBack(new ItemCompletedEventArgs<t> { Error = nullex, Result = default(t) });
            //}

            #region catch WebException
            // The Proxomo Web Service throws exceptions of type WebException that gives the caller (such as this SDK) detailed information when the service was 
            // not able to process a particular request. However, the current behavior of the GetResponseStream earlier in this function is to itself throw a more 
            // general exception ("The remote server returned an error: NotFound.") when it sees that the server (in this case, our Proxomo service) returns 
            // its detailed WebException. However, the actual detailed WebExpection from the Proxomo service is not lost: it can be found buried inside 
            // the more general exception (namely, inside its 'response' property). Therefore, here we catch any such general WebExceptions and extract 
            // the more specific WebException returned by the Proxomo service and make it easy to see in the arguments that are returned to the caller's delegate.
            catch (WebException webex)
            {
                args.IsError = true;
                args.Error = webex;

                WebResponse errResp = ((WebException)webex).Response;

                using (Stream respStream = errResp.GetResponseStream())
                {
                    // read the error response

                    using (StreamReader sreader = new StreamReader(respStream))
                    {
                        if (this.Format == CommunicationType.XML)
                        {
                           //args.HttpRespMessage = sreader.ReadToEnd().Replace("<string>", "").Replace("</string>", "");

                            DataContractSerializer ds = new DataContractSerializer(typeof(Error));
                            Error result = (Error)(ds.ReadObject(sreader.BaseStream));
                            ds = null;

                            args.HttpRespMessage = result.Message;
                            args.HttpRespCode = result.Status;
                        }
                        else if (this.Format == CommunicationType.JSON)
                        {
                            //string temp = sreader.ReadToEnd();
                            //temp =  temp.Replace("\"", "");

                            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(Error));
                            Error result = (Error)(ds.ReadObject(sreader.BaseStream));  
                            ds = null;

                            args.HttpRespMessage = result.Message;
                            args.HttpRespCode = result.Status;
                            args.IsError = true;
                            args.Error = webex;
                            args.Result = default(t);
                        }
                        else
                        {
                            args.HttpRespMessage = sreader.ReadToEnd(); // unknown format. At least return entire response in the message
                        }
                    }
                }

                if (!(state.CallBack == null))
                {
                    state.CallBack(args);
                }
                else
                {
                    state.UserCallback(args);
                }
            }
            #endregion

            #region catch Ex
            // For any other type of exceptions we just indicate there was an error and we return the entire exception object.
            catch (Exception ex)
            {
                args.IsError = true;
                args.Error = ex;

                if (!(state.CallBack == null))
                {
                    state.CallBack(args);
                }
                else
                {
                    state.UserCallback(args);
                }
            }
            #endregion

        }

        private t ReturnXML(StreamReader sreader)
        {
            if (typeof(t).Equals(typeof(string)))
            {
                string result = sreader.ReadToEnd().Replace("<string>", "").Replace("</string>", "");

                CultureInfo provider = new CultureInfo("en-US");
                return (t)Convert.ChangeType(result, typeof(t), provider);
            }
            else
            {
                    DataContractSerializer ds = new DataContractSerializer(typeof(t));
                    t result = (t)(ds.ReadObject(sreader.BaseStream));
                    ds = null;
                    return result;
            }
        }

        private t ReturnJSON(StreamReader sreader)
        {
            if (typeof(t).Equals(typeof(string)))
            {
                string result = sreader.ReadToEnd().Replace("\"", "");

                CultureInfo provider = new CultureInfo("fr-FR");
                return (t)Convert.ChangeType(result, typeof(t), provider);
            }
            else
            {
                DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(t));
                t result = (t)(ds.ReadObject(sreader.BaseStream));
                ds = null;
                return result;
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    token = null;
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }

}

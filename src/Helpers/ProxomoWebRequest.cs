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

        //internal void GetDataItem(string url, string method, string contentType, ProxomoCallbackItem callBack)
        //{
        //    GetDataItem(url, method, contentType, string.Empty, callBack);
        //}

        internal void GetDataItemOLD(string url, string method, string contentType, string content, ProxomoCallbackItem callBack, int x)
        {
            RequestStateItem<t> state = new RequestStateItem<t>();

            ContinuationTokens cTokens = new ContinuationTokens("", "");

            state.CallBack = callBack;
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
                state.CallBack(new ItemCompletedEventArgs<t> { Error = ex, Result = default(t) });
            }
        }

        private void GetResponseItem_Callback(IAsyncResult asyncResult)
        {
            RequestStateItem<t> state = (RequestStateItem<t>)asyncResult.AsyncState;



            try
            {
                state.Response = (HttpWebResponse)state.Request.EndGetResponse(asyncResult);

                // Return back to caller the continuation tokens returned by the service response (if any) ... 
                string NextPartitionKeyDescriptor = "NextPartitionKey";
                string NextRowKeyDescriptor = "NextRowKey";

                ContinuationTokens cTokensIfAny = new ContinuationTokens("", "");

                cTokensIfAny.NextPartitionKey = state.Response.Headers[NextPartitionKeyDescriptor];
                cTokensIfAny.NextRowKey = state.Response.Headers[NextRowKeyDescriptor];

                using (Stream resultStream = state.Response.GetResponseStream())
                {
                    using (StreamReader sreader = new StreamReader(resultStream))
                    {
                        state.Request = null;
                        if (this.Format == CommunicationType.XML)
                        {
                            state.CallBack(new ItemCompletedEventArgs<t> { Error = null, Result = ReturnXML(sreader) });
                        }
                        else if (this.Format == CommunicationType.JSON)
                        {
                            
                            if (!(state.CallBack == null))
                            {
                                state.CallBack(new ItemCompletedEventArgs<t> { Error = null, Result = ReturnJSON(sreader), cTokens = cTokensIfAny });
                            }
                            else
                            {
                                state.UserCallback(new ItemCompletedEventArgs<t> { Error = null, Result = ReturnJSON(sreader), cTokens = cTokensIfAny });
                            }
                        }
                        else
                        {
                            state.CallBack(new ItemCompletedEventArgs<t> { Error = null, Result = default(t) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                state.CallBack(new ItemCompletedEventArgs<t> { Error = ex, Result = default(t) });
            }
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

        //private bool returnTrue(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        //{
        //    return true;
        //}

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

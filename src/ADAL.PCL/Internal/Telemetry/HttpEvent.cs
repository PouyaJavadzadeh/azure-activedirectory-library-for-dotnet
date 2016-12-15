﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    internal class HttpEvent : DefaultEvent
    {
        internal string UserAgent { get; set; }

        internal string HttpMethod { get; set; }

        internal HttpStatusCode HttpResponseCode { get; set; }

        internal string HttpQueryParameters { get; set; }

        internal string HttpResponseMethod { get; set; }

        internal string ResponseApiVersion { get; set; }

        internal void ParseQuery(string Query)
        {
            if (Query.Length != 0)
            {
                string QueryShifted = Query.Substring(1, Query.Length - 1);
                string[] result = QueryShifted.Split('&');
                StringBuilder stringbuilder = new StringBuilder();
                foreach (string s in result)
                {
                    if (s.Contains("="))
                    {
                        stringbuilder.Append(s.Split('=')[0]).Append("&");
                    }
                }

                SetEvent(EventConstants.ExtraQueryParameters,
                    stringbuilder.ToString().Substring(0, stringbuilder.Length - 1));
            }
        }

        internal void SetEvent(string eventName, HttpStatusCode eventParameter)
        {
            EventList.Add(new Tuple<string, string>(eventName, eventParameter.ToString()));
        }

        internal override void ProcessEvent(Dictionary<string, string> dispatchMap)
        {
            if (dispatchMap.ContainsKey(EventConstants.HttpEventCount))
            {
                int previousCount = int.Parse(dispatchMap[EventConstants.HttpEventCount]);
                dispatchMap[EventConstants.HttpEventCount] = previousCount++.ToString();
            }
            else
            {
                dispatchMap.Add(EventConstants.HttpEventCount, "1");
            }

            foreach (Tuple<string, string> Event in EventList)
            {
                if (Event.Item1.Equals(EventConstants.HttpStatusCode))
                {
                    dispatchMap[Event.Item1] = Event.Item2;
                }
            }
        }
    }
}
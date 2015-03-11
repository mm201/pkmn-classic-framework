using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.Web
{
    public class WebException : Exception
    {
        public WebException(String message, int responseCode) : base(message)
        {
            ResponseCode = responseCode;
        }

        public WebException(String message) : this(message, 500)
        {

        }

        public WebException(int responseCode) : this(DefaultMessage(responseCode), responseCode)
        {

        }

        public WebException() : this(500)
        {

        }

        public int ResponseCode { get; private set; }

        public static String DefaultMessage(int responseCode)
        {
            switch (responseCode)
            {
                case 400:
                    return "Bad request";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not found";
                case 500:
                default:
                    return "Server error";
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace ServiceLayer.Orders
{
    [Serializable]
    public partial class CheckoutState
    {
        public CheckoutState()
        {
            CustomProperties = new RouteValueDictionary();
        }

        public static string CheckoutStateSessionKey { get { return "AmCheckoutState"; } }

        public string PaymentSummary
        {
            get
            {
                return CustomProperties["_PaymentSummary"] as string;
            }
            set
            {
                CustomProperties["_PaymentSummary"] = value;
            }
        }

        /// <summary>
        /// Indicated whether the page with the payment method selection was skipped during checkout.
        /// </summary>
        public bool IsPaymentSelectionSkipped { get; set; }

        /// <summary>
        /// Use this dictionary for any custom data required along checkout flow.
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; }
    }
}

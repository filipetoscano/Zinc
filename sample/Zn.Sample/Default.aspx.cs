using System;
using System.Collections.Generic;
using Zinc.WebServices.Web;

namespace Zn.Sample
{
    /// <summary />
    public partial class Default : System.Web.UI.Page
    {
        /// <summary />
        protected void Page_Load( object sender, EventArgs e )
        {
            List<WebServiceDescription> services = WebServiceDescription.Load( typeof( Default ) );

            this.ServiceList.DataSource = services;
            this.DataBind();
        }
    }
}
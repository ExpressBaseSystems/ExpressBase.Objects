﻿using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ObjectContainers
{
    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public class EbBotForm : EbControlContainer
    {
        public bool IsUpdate { get; set; }



        public EbBotForm() { }

        public enum Operations
        {
            Access
        }


        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
            string html = "<form id='@name@' class='eb-form'>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html.Replace("@name@", this.Name);
        }
    }
}

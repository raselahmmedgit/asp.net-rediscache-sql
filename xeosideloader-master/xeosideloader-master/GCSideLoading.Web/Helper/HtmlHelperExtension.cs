using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using System.Web;

namespace Microsoft.AspNetCore.Mvc
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">textbox id</param>
        /// <param name="name">textbox name</param>
        /// <param name="placeholder">textbox placeholder</param>
        /// <param name="required">textbox required</param>
        /// <param name="spellcheck">textbox spellcheck</param>
        /// <returns></returns>
        public static IHtmlContent MdcTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string id = null, string name = null, string placeholder = null, bool required = false, bool spellcheck = false)
        {
            var html = String.Empty;

            #region Text Box

            var textBoxHtmlAttributes = new Dictionary<string, object>();

            textBoxHtmlAttributes.Add("class", "mdc-text-field__input mbc-form-control");

            if (!string.IsNullOrEmpty(id))
            {
                textBoxHtmlAttributes.Add("id", id);
            }

            if (!string.IsNullOrEmpty(name))
            {
                textBoxHtmlAttributes.Add("name", name);
            }

            if (!string.IsNullOrEmpty(placeholder))
            {
                textBoxHtmlAttributes.Add("placeholder", placeholder);
            }

            if (required)
            {
                textBoxHtmlAttributes.Add("required", "required");
            }

            if (spellcheck)
            {
                textBoxHtmlAttributes.Add("spellcheck", "true");
            }

            IHtmlContent textBoxForString = htmlHelper.TextBoxFor(expression, textBoxHtmlAttributes);
            string textBox = ConvertIHtmlContentToString(textBoxForString);

            #endregion

            #region Label

            var labelHtmlAttributes = new Dictionary<string, object>();

            labelHtmlAttributes.Add("class", "mdc-floating-label");

            if (!string.IsNullOrEmpty(id))
            {
                labelHtmlAttributes.Add("for", id);
            }

            IHtmlContent labelForString = htmlHelper.LabelFor(expression, labelHtmlAttributes);
            string label = ConvertIHtmlContentToString(labelForString);

            #endregion

            #region Validation Message

            var validHtmlAttributes = new Dictionary<string, object>();

            validHtmlAttributes.Add("class", "mdc-field-validation-error");

            IHtmlContent validationMessageForString = htmlHelper.ValidationMessageFor(expression, "", validHtmlAttributes);
            string validationMessage = ConvertIHtmlContentToString(validationMessageForString);

            #endregion

            html += "<div class='mdc-text-field mdc-text-field--outlined'>";

            html += textBox;
            html += "<div class='mdc-notched-outline'>";

            html += "<div class='mdc-notched-outline__leading'></div>";
            html += "<div class='mdc-notched-outline__notch'>";
            html += label;
            html += "</div>";
            html += "<div class='mdc-notched-outline__trailing'></div>";

            html += "</div>";

            html += "</div>";

            html += validationMessage;

            return new HtmlString(html);
        }

        private static string ConvertIHtmlContentToString(IHtmlContent htmlContent)
        {
            using (var stringWriter = new StringWriter())
            {
                htmlContent.WriteTo(stringWriter, HtmlEncoder.Default);
                return stringWriter.ToString();
            }
        }
    }
}


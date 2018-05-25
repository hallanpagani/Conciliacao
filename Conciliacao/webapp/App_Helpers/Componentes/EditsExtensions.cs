using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Conciliacao.App_Helpers.Componentes
{
    public static class EditsExtensions
    {
        #region edits
        public static MvcHtmlString TextBoxPadraoFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.TextBoxPadraoFor(expression, htmlAttributes);
        }

        public static MvcHtmlString TextBoxPadraoFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var dict = new RouteValueDictionary(htmlAttributes);
            return html.TextBoxPadraoFor(expression, dict);
        }

        public static MvcHtmlString TextBoxPadraoFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string id = ExpressionHelper.GetExpressionText(expression).Split('.').Last().ToLower();
            string nome = id;
            //string placeHolder = metadata.Description ?? id;

            bool isRequired = false;

            if (metadata.ContainerType != null)
            {
                isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
                                .GetCustomAttributes(typeof(RequiredAttribute), false)
                                .Length == 1;
            }

            TimestampAttribute time = (TimestampAttribute)
                    Attribute.GetCustomAttribute(
                        (expression.Body as MemberExpression ??
                         ((UnaryExpression)expression.Body).Operand as MemberExpression).Member,
                        typeof(TimestampAttribute));

            DataTypeAttribute date = (DataTypeAttribute)
                    Attribute.GetCustomAttribute(
                        (expression.Body as MemberExpression ??
                         ((UnaryExpression)expression.Body).Operand as MemberExpression).Member,
                        typeof(DataTypeAttribute));

            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            string classe = "";
            string tamanho = "col-sm-12";
            string group_addon = "";

            if ((expression.ReturnType == typeof(decimal)) || (expression.ReturnType == typeof(decimal?)))
            {
                group_addon = "<span class='input-group-addon'> R$ </span>";
                classe = "form-control text-right valor";  //input-lg
                var placeHolder = "0,00";
                htmlAttributes.Add("placeholder", placeHolder);
            }
            else if ((date != null) && Equals(date.GetDataTypeName(),"Date"))
            {
                if (!htmlAttributes.ContainsKey("filtrar"))
                {
                    group_addon = "<span class='input-group-addon'><i class='fa fa-calendar'></i></span>";
                }
                classe = "form-control datepicker "; // input-lg
                htmlAttributes.Add("maxlength", "10");
                htmlAttributes.Add("data_dateformat", "dd/mm/yy");
                htmlAttributes.Add("onkeypress", "mascaraDT(this, DATA)");
            }
            else if (time != null)
            {
                group_addon = "<span class='input-group-addon'><i class='fa fa-clock-o'></i></span>";
                classe = "form-control "; // input-lg
                htmlAttributes.Add("maxlength", "5");
            }
            else
            {
                classe = classe + "form-control "; // input-lg
            }

            htmlAttributes.Add("Name", nome.ToLower());
            htmlAttributes.Add("id", id);
            
            if (isRequired == true)
            {
                if (classe == "")
                {
                    classe = "requerido";
                }
                else
                {
                    classe = classe + " requerido";
                }
            }
            htmlAttributes.Add("class", classe);

            if (htmlAttributes.ContainsKey("tamanho"))
            {
                tamanho += " "+htmlAttributes["tamanho"];
            }
            htmlAttributes.Add("style", "text-transform:uppercase");


            if ((expression.ReturnType == typeof(decimal)) || (expression.ReturnType == typeof(decimal?)))
            {
                /// refatorar 
                StringBuilder str = new StringBuilder();
                str.Append("<div class='input-group'>");
                str.Append(html.TextBoxFor(expression, htmlAttributes));
                str.Append(group_addon);
                str.Append("</div>");

                return new MvcHtmlString(str.ToString());

            } else if ( ((date != null) && Equals(date.GetDataTypeName(), "Date")) || (time != null) )
            {
                /// refatorar 
                StringBuilder str = new StringBuilder();
                str.Append("<div class='input-group'>");
                str.Append(html.TextBoxFor(expression, htmlAttributes));
                str.Append(group_addon);
                str.Append("</div>");

                return new MvcHtmlString(str.ToString());

            } else {
                /// refatorar 
                StringBuilder str = new StringBuilder();
          //      str.Append("<div class='col-md-10'>");
          //      str.Append("<div class='row'>");
        //        str.Append("<div class='"+ tamanho+ "'>");
                str.Append(html.TextBoxFor(expression, htmlAttributes));
          //      str.Append("</div>");
          //      str.Append("</div>");
         //       str.Append("</div>");
                return new MvcHtmlString(str.ToString());
            }

        }
        #endregion

        #region select
        public static MvcHtmlString SelectPadraoFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.SelectPadraoFor(expression, htmlAttributes);
        }

        public static MvcHtmlString SelectPadraoFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var dict = new RouteValueDictionary(htmlAttributes);
            return html.SelectPadraoFor(expression, dict);
        }

        public static MvcHtmlString SelectPadraoFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string id = ExpressionHelper.GetExpressionText(expression).Split('.').Last().ToLower();
            string nome = id;
            //string placeHolder = metadata.Description ?? id;

            bool isRequired = false;

            if (metadata.ContainerType != null)
            {
                isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
                                .GetCustomAttributes(typeof(RequiredAttribute), false)
                                .Length == 1;
            }

            DataTypeAttribute date = (DataTypeAttribute)
                    Attribute.GetCustomAttribute(
                        (expression.Body as MemberExpression ??
                         ((UnaryExpression)expression.Body).Operand as MemberExpression).Member,
                        typeof(DataTypeAttribute));

            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
          //  htmlAttributes.Add("Name", nome.ToLower());
          //  htmlAttributes.Add("id", id);
            htmlAttributes.Add("style", "width:100%");
            StringBuilder str = new StringBuilder();
            str.Append("<div class='col-md-10'>");
            str.Append("<div class='row'>");
            str.Append("<div class='col-sm-12'>");
            str.Append(html.TextBoxFor(expression, htmlAttributes));
            str.Append("</div>");
            str.Append("</div>");
            str.Append("</div>");
            return new MvcHtmlString(str.ToString());

        }
        #endregion
    }
}
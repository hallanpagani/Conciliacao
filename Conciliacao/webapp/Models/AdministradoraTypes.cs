using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models
{
    public class AdministradoraTypes
    {
        public static List<SelectListItem> getAdministradoras(string Selecionar)
        {
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = "", Value = "" , Selected = Selecionar.Equals("") });
            data.Add(new SelectListItem() { Text = "AMEX", Value = "AMEX", Selected = Selecionar.Equals("AMEX") });
            data.Add(new SelectListItem() { Text = "BANESE", Value = "BANESE", Selected = Selecionar.Equals("BANESE") });
            data.Add(new SelectListItem() { Text = "BANESCARD", Value = "BANESCARD", Selected = Selecionar.Equals("BANESCARD") });
            data.Add(new SelectListItem() { Text = "CABAL", Value = "CABAL", Selected = Selecionar.Equals("CABAL") });
            data.Add(new SelectListItem() { Text = "CREDSYSTEM", Value = "CREDSYSTEM", Selected = Selecionar.Equals("CREDSYSTEM") });
            data.Add(new SelectListItem() { Text = "CREDZ", Value = "CREDZ", Selected = Selecionar.Equals("CREDZ") });
            data.Add(new SelectListItem() { Text = "CUP", Value = "CUP", Selected = Selecionar.Equals("CUP") });
            data.Add(new SelectListItem() { Text = "DINERS CLUB", Value = "DINERS CLUB", Selected = Selecionar.Equals("DINERS CLUB") });
            data.Add(new SelectListItem() { Text = "ELO", Value = "ELO", Selected = Selecionar.Equals("ELO") });
            data.Add(new SelectListItem() { Text = "HIPERCARD", Value = "HIPERCARD", Selected = Selecionar.Equals("HIPERCARD") });
            data.Add(new SelectListItem() { Text = "JCB", Value = "JCB", Selected = Selecionar.Equals("JCB") });
            data.Add(new SelectListItem() { Text = "MAESTRO", Value = "MAESTRO", Selected = Selecionar.Equals("MAESTRO") });
            data.Add(new SelectListItem() { Text = "MASTERCARD", Value = "MASTERCARD", Selected = Selecionar.Equals("MASTERCARD") });
            data.Add(new SelectListItem() { Text = "SICRED", Value = "SICRED", Selected = Selecionar.Equals("SICRED") });
            data.Add(new SelectListItem() { Text = "SOROCRED", Value = "SOROCRED", Selected = Selecionar.Equals("SOROCRED") });
            data.Add(new SelectListItem() { Text = "VISA", Value = "VISA ", Selected = Selecionar.Equals("VISA") });
            data.Add(new SelectListItem() { Text = "VISA ELECTRON", Value = "VISA ELECTRON", Selected = Selecionar.Equals("VISA ELECTRON") });
            return data;
        }

        public static List<SelectListItem> getTpOperacoes(string Selecionar)
        {
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = "", Value = "", Selected = Selecionar.Equals("") });
            data.Add(new SelectListItem() { Text = "DÉBITO", Value = "DÉBITO", Selected = Selecionar.Equals("DÉBITO") });
            data.Add(new SelectListItem() { Text = "CRÉDITO", Value = "CRÉDITO", Selected = Selecionar.Equals("CRÉDITO") });
            return data;
        }

    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Web;
using System.Web.Security;

namespace ConciliacaoModelo.model.generico
{
    public class BaseUGrav : BaseID
    { 
        [Required]
        [Column("id_usuario")]
        public long Usuario { get; set; }

        public BaseUGrav()
        {
            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            try
            {
                Usuario = claimsIdentity != null ? Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier ?? "0").Value) : 0;
            }
            catch (Exception)
            {
                Usuario = 0;
            }
        }
    }
}

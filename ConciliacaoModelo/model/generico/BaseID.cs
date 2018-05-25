using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Web;

namespace ConciliacaoModelo.model.generico
{
    public class BaseID
    {
        [Required]
        [Column("id_conta")]
        public long IdConta { get; set; }

        public BaseID()
        {
            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            try
            {
                IdConta = claimsIdentity != null ? Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.GroupSid).Value ?? "0") : 0;
            }
            catch (Exception)
            {
                IdConta = 0;
            }
        }

    }


}

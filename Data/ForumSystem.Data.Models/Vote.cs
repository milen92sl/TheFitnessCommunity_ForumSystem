using ForumSystem.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSystem.Data.Models
{
    public class Vote : BaseModel<int>
    {
        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public VoteType Type { get; set; }
    }
}

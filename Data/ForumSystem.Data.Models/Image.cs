namespace ForumSystem.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using ForumSystem.Data.Common.Models;

    public class Image : BaseModel<int>
    {
        public string Url { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }
    }
}

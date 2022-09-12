using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETMVC.Models
{
    public class Summernote
    {
        public Summernote(string id, bool load = true)
        {
            LoadLibrary = load;
            IDEditor = id;
        }
        public bool LoadLibrary { set; get; }
        public string IDEditor { set; get; }
        public int Height { set; get; } = 120;
        public string toolBar { set; get; } = @"
        [
                ['style', ['bold', 'italic', 'underline', 'clear']],
                ['font', ['strikethrough', 'superscript', 'subscript']],
                ['fontsize', ['fontsize']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['height', ['height']]
            ]
        ";
    }
}

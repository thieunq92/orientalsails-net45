﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.DataTransferObject
{
    public class DocumentCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<DocumentDTO> ListDocumentDTO { get; set; }
        public class DocumentDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string Url { get; set; }
        }
    }
}
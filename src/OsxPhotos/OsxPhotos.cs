using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OsxPhotos
{
    public class PhotoCollection
    {
        public List<Photo> Photos { get; set; }

        public static PhotoCollection Deserialize(string json)
        {
            var photoList = JsonSerializer.Deserialize<List<Photo>>(json);
            var photos = new PhotoCollection { Photos = photoList };
            return photos;
        }
    }

    public class Photo
    {
        public string[] albums { get; set; }
        public bool burst { get; set; }
        public PhotoComment[] comments {get;set;}
        public DateTime date { get; set; }
        public DateTime? date_modified { get; set; }
        public string description { get; set; }
        // public Exif exif {get;set;}
        public bool external_edit { get; set; }
        // public Face[] faces {get;set;}
        public bool favorite { get; set; }
        public string filename { get; set; }
        // public Dictionary<string, string[]> folders {get;set;}
        public bool has_raw { get; set; }
        public bool hasadjustments { get; set; }
        public bool hdr { get; set; }
        public int height { get; set; }
        public bool hidden { get; set; }
        // public string incloud {get;set;}
        public bool intrash { get; set; }
        public bool iscloudasset { get; set; }
        public bool ismissing { get; set; }
        public bool ismovie { get; set; }
        public bool isphoto { get; set; }
        public bool israw { get; set; }
        public bool isreference { get; set; }
        public string[] keywords { get; set; }
        public string[] labels { get; set; }
        // public string latitude {get;set;}
        public string library { get; set; } // library path
        // public object[] likes {get;set;}
        public bool live_photo { get; set; }
        // public string longitude {get;set;}
        // public int orientation {get;set;}
        public string original_filename { get; set; }
        public int original_filesize { get; set; }
        public int original_height { get; set; }
        public int original_orientation { get; set; }
        public int original_width { get; set; }
        public bool panorama { get; set; }

        public string path { get; set; } // absolute path to file in library {get;set;}
        public string path_edited { get; set; }
        public string path_live_photo { get; set; }
        public string path_raw { get; set; }
        // public Person[] persons {get;set;}
        // public Dictionary<string, Place> place {get;set;}
        public bool portrait { get; set; }
        public bool raw_original { get; set; }
        // public Score score {get;set;}
        public bool screenshot { get; set; }
        // public SearchInfo search_info {get;set;}
        public bool selfie { get; set; }
        public bool shared { get; set; }
        public bool slow_mo { get; set; }
        public bool time_lapse { get; set; }
        public string title { get; set; }
        public string uti { get; set; }
        public string uti_original { get; set; }
        public string uti_raw { get; set; }
        public string uuid { get; set; }
        public int width { get; set; }
        public string ExportedFilename
        {
            get
            {
                var exported = Path.GetFileNameWithoutExtension(this.original_filename);
                return exported + ".jpg";
            }
        }
    }

    public class PhotoComment
    {
        public string user { get; set; }
        public string text { get; set; }
        public bool ismine { get; set; }
        public DateTime datetime { get; set; }
    }
}
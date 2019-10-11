using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Activities;
using System.Activities.Tracking;

namespace ActivityLibraryCore.Compression
{
    public sealed class ZipDirectory : CodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> Active { get; set; }
        [RequiredArgument]
        public InArgument<string> DirectoryPath { get; set; }
        [RequiredArgument]
        public InArgument<string> ZipFileName { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            if (!Active.Get(context))
                return;

            try
            {
                string DirPath = DirectoryPath.Get(context);
                string ZipPath = ZipFileName.Get(context);

                ZipFile.CreateFromDirectory(DirPath, ZipPath);
            }
            catch (Exception ex)
            {
                var record = new CustomTrackingRecord("Warning");
                record.Data.Add(new KeyValuePair<string, object>("Message", "Error while zipping.\n" + ex.ToString()));
                context.Track(record);
            }
        }
    }
}

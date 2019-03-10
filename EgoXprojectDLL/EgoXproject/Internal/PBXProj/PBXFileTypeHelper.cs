//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.IO;

namespace Egomotion.EgoXproject.Internal
{

    internal static class PBXFileTypeHelper
    {
        public static PBXFileType FileTypeFromFileName(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return FileTypeFromExtension(ext);
        }

        public static PBXFileType FileTypeFromExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return PBXFileType.File;
            }

            string ext = extension.ToLower();

            if (ext.StartsWith(".", System.StringComparison.InvariantCultureIgnoreCase))
            {
                ext = ext.Substring(1);
            }

            if (string.IsNullOrEmpty(ext))
            {
                //TODO throw exception?
                return PBXFileType.File;
            }

            if (ext == "framework")
            {
                return PBXFileType.WrapperFramework;
            }

            if (ext == "app")
            {
                return PBXFileType.WrapperApplication;
            }

            if (ext == "appex")
            {
                return PBXFileType.WrapperAppExtension;
            }

            if (ext == "bundle")
            {
                return PBXFileType.WrapperPlugIn;
            }

            if (ext == "xctest" ||
                    ext == "octest")
            {
                //TODO other types
                return PBXFileType.WrapperCFBundle;
            }

            if (ext == "xcdatamodeld" ||
                    ext == "xcdatamodel")
            {
                return PBXFileType.WrapperXCDataModel;
            }

            if (ext == "xcmappingmodel")
            {
                return PBXFileType.WrapperXCMappingModel;
            }

            if (ext == "dylib")
            {
                return PBXFileType.CompiledMachODylib;
            }

            if (ext == "a")
            {
                return PBXFileType.ArchiveAr;
            }

            if (ext == "zip")
            {
                return PBXFileType.ArchiveZip;
            }

            if (ext == "c")
            {
                return PBXFileType.SourcecodeC;
            }

            if (ext == "h" ||
                    ext == "pch")
            {
                return PBXFileType.SourcecodeCHeader;
            }

            if (ext == "cpp" ||
                    ext == "cc" ||
                    ext == "cxx")
            {
                return PBXFileType.SourcecodeCpp;
            }

            if (ext == "hpp" ||
                    ext == "hh" ||
                    ext == "hxx")
            {
                return PBXFileType.SourcecodeCppHeader;
            }

            if (ext == "m")
            {
                return PBXFileType.SourcecodeObjectiveC;
            }

            if (ext == "mm")
            {
                return PBXFileType.SourcecodeObjectiveCpp;
            }

            if (ext == "s" ||
                    ext == "asm")
            {
                return PBXFileType.SourceCodeAsm;
            }

            if (ext == "js")
            {
                return PBXFileType.SourcecodeJavascript;
            }

            if (ext == "swift")
            {
                return PBXFileType.SourcecodeSwift;
            }

            if (ext == "metal")
            {
                return PBXFileType.SourcecodeMetal;
            }

            if (ext == "fsh" ||
                    ext == "vsh")
            {
                return PBXFileType.SourcecodeGLSL;
            }

            if (ext == "tbd")
            {
                return PBXFileType.SourcecodeTextBasedDylibDefinition;
            }

            if (ext == "png")
            {
                return PBXFileType.ImagePng;
            }

            if (ext == "jpg" ||
                    ext == "jpeg")
            {
                return PBXFileType.ImageJpeg;
            }

            if (ext == "pdf")
            {
                return PBXFileType.ImagePdf;
            }

            if (ext == "pict" ||
                    ext == "pct")
            {
                return PBXFileType.ImagePict;
            }

            if (ext == "tiff" ||
                    ext == "tif")
            {
                return PBXFileType.ImageTiff;
            }

            if (ext == "bmp")
            {
                return PBXFileType.ImageBmp;
            }

            if (ext == "gif")
            {
                return PBXFileType.ImageGif;
            }

            if (ext == "icns")
            {
                return PBXFileType.ImageIcns;
            }

            if (ext == "ico")
            {
                return PBXFileType.ImageIco;
            }

            if (ext == "txt" ||
                    ext == "csv" ||
                    ext == "geojson" ||
                    ext == "apns")
            {
                //TODO more
                return PBXFileType.Text;
            }

            if (ext == "plist")
            {
                return PBXFileType.TextPListXML;
            }

            if (ext == "entitlements")
            {
                return PBXFileType.TextPListEntitlements;
            }

            if (ext == "strings")
            {
                return PBXFileType.TextPlistStrings;
            }

            if (ext == "html" ||
                    ext == "htm")
            {
                return PBXFileType.TextHtml;
            }

            if (ext == "css")
            {
                return PBXFileType.TextCss;
            }

            if (ext == "xml" ||
                    ext == "gpx")
            {
                return PBXFileType.TextXml;
            }

            if (ext == "rtf")
            {
                return PBXFileType.TextRtf;
            }

            if (ext == "xcconfig")
            {
                return PBXFileType.TextXCConfig;
            }

            if (ext == "sh")
            {
                return PBXFileType.TextScriptShell;
            }

            if (ext == "mp3")
            {
                return PBXFileType.AudioMp3;
            }

            if (ext == "wav")
            {
                return PBXFileType.AudioWav;
            }

            if (ext == "avi")
            {
                return PBXFileType.VideoAvi;
            }

            if (ext == "mov")
            {
                return PBXFileType.VideoQuicktime;
            }

            if (ext == "mpg" ||
                    ext == "mpeg")
            {
                return PBXFileType.VideoQuicktime;
            }

            if (ext == "xcassets")
            {
                return PBXFileType.FolderAssetCatalog;
            }

            if (ext == "xib")
            {
                return PBXFileType.FileXib;
            }

            if (ext == "storyboard")
            {
                return PBXFileType.FileStoryboard;
            }

            if (ext == "playground")
            {
                return PBXFileType.FilePlayground;
            }

            //          if (ext == "") {
            //              return PBXFileType.;
            //          }
            return PBXFileType.File;
        }

        public static bool IsSourceCodeFile(PBXFileType type)
        {
            return (type == PBXFileType.SourceCodeAsm ||
                    type == PBXFileType.SourcecodeC ||
                    type == PBXFileType.SourcecodeCpp ||
                    type == PBXFileType.SourcecodeObjectiveC ||
                    type == PBXFileType.SourcecodeObjectiveCpp ||
                    type == PBXFileType.SourcecodeSwift ||
                    type == PBXFileType.WrapperXCDataModel ||
                    type == PBXFileType.WrapperXCMappingModel ||
                    type == PBXFileType.SourcecodeMetal);
        }

        public static bool IsHeaderFile(PBXFileType type)
        {
            return (type == PBXFileType.SourcecodeCHeader || type == PBXFileType.SourcecodeCppHeader);
        }

        public static bool IsFramework(PBXFileType type)
        {
            return (type == PBXFileType.WrapperFramework);
        }

        public static bool IsLibrary(PBXFileType type)
        {
            return (type == PBXFileType.ArchiveAr ||
                    type == PBXFileType.CompiledMachODylib ||
                    type == PBXFileType.SourcecodeTextBasedDylibDefinition);
        }

        public static bool IsFrameworkOrLibrary(PBXFileType type)
        {
            return IsFramework(type) || IsLibrary(type);
        }

        public static bool IsContainer(PBXFileType type)
        {
            return (type == PBXFileType.FolderAssetCatalog ||
                    type == PBXFileType.WrapperXCDataModel ||
                    type == PBXFileType.WrapperXCMappingModel ||
                    type == PBXFileType.WrapperPlugIn ||
                    type == PBXFileType.WrapperFramework ||
                    type == PBXFileType.WrapperApplication ||
                    type == PBXFileType.WrapperAppExtension ||
                    type == PBXFileType.WrapperCFBundle ||
                    type == PBXFileType.FilePlayground);
        }

        public static bool IsNotBuilt(PBXFileType type)
        {
            return (IsHeaderFile(type) ||
                    type == PBXFileType.FilePlayground ||
                    type == PBXFileType.TextXCConfig ||
                    type == PBXFileType.TextScriptShell);
        }

        public static bool IsResourceFile(PBXFileType type)
        {
            return (!IsSourceCodeFile(type) && !IsNotBuilt(type));
        }

        public static bool IsApplication(PBXFileType type)
        {
            return (type == PBXFileType.WrapperApplication ||
                    type == PBXFileType.WrapperAppExtension);
        }

        public static bool IsCoreDatamodel(PBXFileType type)
        {
            return type == PBXFileType.WrapperXCDataModel;
        }

        public static bool IsEntitlementsFile(string fileName)
        {
            return Path.GetExtension(fileName) == ".entitlements";
        }
    }
}

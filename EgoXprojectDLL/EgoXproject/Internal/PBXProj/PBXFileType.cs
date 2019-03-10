//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System.ComponentModel;

namespace Egomotion.EgoXproject.Internal
{
    internal enum PBXFileType
    {
        [XcodeDataValue("file")]        // .aif .aiff
        File,

        //wrappers
        [XcodeDataValue("wrapper.framework")]
        WrapperFramework,

        [XcodeDataValue("wrapper.application")]
        WrapperApplication,

        [XcodeDataValue("wrapper.app-extension")]
        WrapperAppExtension,

        [XcodeDataValue("\"wrapper.plug-in\"")]
        WrapperPlugIn,

        [XcodeDataValue("wrapper.cfbundle")]
        WrapperCFBundle,

        [XcodeDataValue("wrapper.xcdatamodel")]
        WrapperXCDataModel,

        [XcodeDataValue("wrapper.xcmappingmodel")]
        WrapperXCMappingModel,

        //compiled
        [XcodeDataValue("\"compiled.mach-o.dylib\"")]
        CompiledMachODylib,


        //archives
        [XcodeDataValue("archive.ar")]
        ArchiveAr,

        [XcodeDataValue("archive.zip")]
        ArchiveZip,

        //src
        [XcodeDataValue("sourcecode.c.h")]
        SourcecodeCHeader,

        [XcodeDataValue("sourcecode.c.c")]
        SourcecodeC,

        [XcodeDataValue("sourcecode.c.objc")]
        SourcecodeObjectiveC,

        [XcodeDataValue("sourcecode.cpp.objcpp")]
        SourcecodeObjectiveCpp,

        [XcodeDataValue("sourcecode.cpp.cpp")]
        SourcecodeCpp,

        [XcodeDataValue("sourcecode.cpp.h")]
        SourcecodeCppHeader,

        [XcodeDataValue("sourcecode.asm")]
        SourceCodeAsm,

        [XcodeDataValue("sourcecode.javascript")]
        SourcecodeJavascript,

        [XcodeDataValue("sourcecode.swift")]
        SourcecodeSwift,

        [XcodeDataValue("sourcecode.metal")]
        SourcecodeMetal,

        [XcodeDataValue("sourcecode.glsl")]
        SourcecodeGLSL,

        [XcodeDataValue("sourcecode.text-based-dylib-definition")]
        SourcecodeTextBasedDylibDefinition,

        //images
        [XcodeDataValue("image.png")]
        ImagePng,

        [XcodeDataValue("image.jpeg")]
        ImageJpeg,

        [XcodeDataValue("image.pdf")]
        ImagePdf,

        [XcodeDataValue("image.pict")]
        ImagePict,

        [XcodeDataValue("image.tiff")]
        ImageTiff,

        [XcodeDataValue("image.bmp")]
        ImageBmp,

        [XcodeDataValue("image.gif")]
        ImageGif,

        [XcodeDataValue("image.icns")]
        ImageIcns,

        [XcodeDataValue("icon.ico")]
        ImageIco,

        //text
        [XcodeDataValue("text")]        //.txt, .csv
        Text,

        [XcodeDataValue("text.plist.xml")]
        TextPListXML,

        [XcodeDataValue("text.plist.entitlements")]
        TextPListEntitlements,

        [XcodeDataValue("text.plist.strings")]
        TextPlistStrings,

        [XcodeDataValue("text.html")]
        TextHtml,

        [XcodeDataValue("text.css")]
        TextCss,

        [XcodeDataValue("text.rtf")]
        TextRtf,

        [XcodeDataValue("text.xml")]
        TextXml,

        [XcodeDataValue("text.script.sh")]
        TextScriptShell,

        [XcodeDataValue("text.xcconfig")]
        TextXCConfig,


        //audio
        [XcodeDataValue("audio.mp3")]
        AudioMp3,

        [XcodeDataValue("audio.wav")]
        AudioWav,

        //video
        [XcodeDataValue("video.avi")]
        VideoAvi,
        [XcodeDataValue("video.quicktime")]
        VideoQuicktime,
        [XcodeDataValue("video.mpeg")]
        VideoMpeg,


        //folder
        [XcodeDataValue("folder.assetcatalog")]
        FolderAssetCatalog,


        //file
        [XcodeDataValue("file.xib")]
        FileXib,

        [XcodeDataValue("file.storyboard")]
        FileStoryboard,

        [XcodeDataValue("file.playground")]
        FilePlayground
    }

    ;

}

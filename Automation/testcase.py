# -*- coding: utf-8 -*-
import os
from os import path
import testmodule


@testmodule.testcase
def testTextPost(app):
    app.StartComposePost()
    app.TypeTextInPost("create a simple text post")
    app.FinishComposePost()
    app.ViewFirstPost()
    app.AddComment("comment on simple text post")


@testmodule.testcase
def testPhotoPost(app):
    app.StartComposePost()
    app.TypeTextInPost("add 1 photo, remove 1 photo, and add 2 photos, then save them all to one folder")
    app.AddPhotosToPost(
        photolist=[
            r"C:\Users\kchiu\Dropbox\Camera Uploads\2011-12-25 17.28.10.jpg"
        ]
    )
    app.RemovePhotosFromPost()
    app.AddPhotosToPost(
        photolist=[
            r"C:\Users\kchiu\Dropbox\Suitcase\Testing Data\Image Orientation\86aa1a18-ff71-4e1c-9bd6-7f0a282f1c18.jpg",
            r"C:\Users\kchiu\Dropbox\Camera Uploads\2011-12-25 17.28.10.jpg"
        ],
        hasremoved=True
    )
    app.FinishComposePost(photonum=2)
    app.ViewFirstPost(photonum=2)
    app.ViewFirstPhoto()
    app.ViewNextPhoto()
    app.SaveAllToDesktop(folder="allphoto")


@testmodule.testcase
def testCopyPasteURLTextPost(app):
    app.StartComposePost()
    app.CopyPasteURLTextInPost(url="http://ushijima.tumblr.com/", haspreview=True, text=u"新一代Waveface性感女神 http://ushijima.tumblr.com/")
    app.ClickNextThumbnail()
    app.ClickNextThumbnail()
    app.FinishComposePost()
    app.ViewFirstPost()
    app.PageDownExplorer()
    app.PageDownExplorer()
    app.PageDownExplorer()
    app.PageDownExplorer()
    app.PageDownExplorer()
    app.PageUpExplorer()
    app.PageUpExplorer()


if os.environ.get("PROGRAMFILES(X86)"):
    PROGRAMFILES = os.environ["PROGRAMFILES(X86)"]
else:
    PROGRAMFILES = os.environ["PROGRAMFILES"]

waveface = testmodule.App(path.join(PROGRAMFILES, "WavefaceStation/WavefaceWindowsClient.exe"))
waveface.Start()
waveface.Login(password="novirus")
testTextPost(waveface)
testPhotoPost(waveface)
testCopyPasteURLTextPost(waveface)
waveface.Close()

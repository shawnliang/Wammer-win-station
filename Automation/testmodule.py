# -*- coding: utf-8 -*-
import time
import subprocess
import shlex
from functools import wraps
from pywinauto import application
from pywinauto import timings

timings.Timings.closeclick_dialog_close_wait = 10  # wait long time after close window
timings.Timings.after_sendkeys_key_wait = 0.05  # type key too fast would cause client crash


def testcase(func):
    @wraps(func)
    def wrapper(app):
        app.TestCase = func.__name__
        return func(app)
    return wrapper


class ScreenRecorder(object):
    def __init__(self, basename):
        self.step = 1
        self.basename = basename

    def takeshot(self, comment):
        subprocess.check_call(shlex.split("nircmd.exe savescreenshot {0}_{1}_{2}.png".format(self.basename, self.step, comment)))
        self.step += 1


class App(object):
    def __init__(self, apppath):
        self.apppath = apppath
        self._testcase = None
        self._screenrecorder = None

    def Start(self):
        self.app = application.Application.start(self.apppath)
        self.TakeShot("StartProgram")

    def Close(self):
        self.app["Waveface"].Close()
        self.TakeShot("CloseProgram")

    def getT(self):
        return self._testcase

    def setT(self, testcase):
        self._testcase = testcase
        self._screenrecorder = ScreenRecorder(testcase)

    def delT(self):
        del self._testcase
        del self._screenrecorder

    TestCase = property(getT, setT, delT, "Application's Test Case")

    def TakeShot(self, comment, delay=0.5):
        if self._screenrecorder:
            time.sleep(delay)
            self._screenrecorder.takeshot(comment)

    def Login(self, email=None, password=None, rememberpassword=False):
        if email:
            self.app["Login Waveface"]["EmailEdit"].TypeKeys(email)
        if password:
            self.app["Login Waveface"]["PasswordEdit"].TypeKeys(password)
        if rememberpassword:
            self.app["Login Waveface"]["Remember password"].Click()
        self.app["Login Waveface"]["Login"].Click()
        if not self.app["Waveface"].Exists(timeout=10):
            raise Exception("Unable to see main window")
        time.sleep(10)  # bad: need to wait for long time after login
        self.TakeShot("LoginSuccess")

    def StartComposePost(self):
        self.app["Waveface"]["Create a New Post"].Click()
        self.TakeShot("StartComposePost")

    def TypeTextInPost(self, text):
        self.app["Create a New Post"]["WindowsForms10.RichEdit20W.app.0.360e0332"].TypeKeys(text)
        self.TakeShot("TypeTextInPost")

    def TypeURLTextInPost(self, url, haspreview, text):
        self.app["Create a New Post"]["WindowsForms10.RichEdit20W.app.0.360e0332"].TypeKeys(text)
        self.TakeShot("TypeURLTextInPost")
        if haspreview:
            self.app["Create a New Post"]["Web link preview for {0}".format(url)].Exists(timeout=5)
            self.TakeShot("HasWebPreview")
        else:
            self.app["Create a New Post"]["No web preview for {0}".format(url)].Exists(timeout=5)
            self.TakeShot("NoWebPreview")

    def CopyPasteTextInPost(self, text):
        notepad = application.Application.start("Notepad.exe")
        notepad["Notepad"]["Edit"].SetEditText(text)
        notepad["Notepad"].TypeKeys("^a")
        notepad["Notepad"].TypeKeys("^c")
        notepad["Notepad"].TypeKeys("{DEL}")
        notepad["Notepad"].Close()
        self.app["Create a New Post"]["WindowsForms10.RichEdit20W.app.0.360e0332"].TypeKeys("^v")
        self.TakeShot("CopyPasteTextInPost")

    def CopyPasteURLTextInPost(self, url, haspreview, text):
        notepad = application.Application.start("Notepad.exe")
        notepad["Notepad"]["Edit"].SetEditText(text)
        notepad["Notepad"].TypeKeys("^a")
        notepad["Notepad"].TypeKeys("^c")
        notepad["Notepad"].TypeKeys("{DEL}")
        notepad["Notepad"].Close()
        self.app["Create a New Post"]["WindowsForms10.RichEdit20W.app.0.360e0332"].TypeKeys("^v")
        self.TakeShot("CopyPasteURLTextInPost")
        if haspreview:
            self.app["Create a New Post"]["Web link preview for {0}".format(url)].Exists(timeout=5)
            self.TakeShot("HasWebPreview", 1.5)
        else:
            self.app["Create a New Post"]["No web preview for {0}".format(url)].Exists(timeout=5)
            self.TakeShot("NoWebPreview", 1.5)

    def AddPhotosToPost(self, photolist, hasremoved=False):
        if hasremoved:
            addphotobtn = "Add Photo2"
        else:
            addphotobtn = "Add Photo0"

        index = 1
        for photo in photolist:
            self.app["Create a New Post"][addphotobtn].ClickInput()
            self.app["Create a New Post"][addphotobtn].Click()
            self.app["Open"]["FilenameEdit"].TypeKeys(photo)
            self.app["Open"].TypeKeys("{ENTER}")
            self.TakeShot("Add{0}PhotoToPost".format(index))
            index += 1

    def RemovePhotosFromPost(self):
        self.app["Create a New Post"]["Remove"].Click()
        self.app.top_window_()["Yes"].Click()

    def ClickNoThumbnail(self):
        self.app["Create a New Post"]["No Thumbnail"].Click()
        self.TakeShot("ClickNoThumbnail")

    def ClickPrevThumbnail(self):
        self.app["Create a New Post"]["<"].Click()
        self.TakeShot("ClickPrevThumbnail")

    def ClickNextThumbnail(self):
        self.app["Create a New Post"][">"].Click()
        self.TakeShot("ClickNextThumbnail")

    def FinishComposePost(self, photonum=0):
        self.app["Create a New Post"]["Create2"].Click()
        count = 0
        while not self.app["Waveface"]["Post Success"].Exists(timeout=5) and count < photonum:
            self.TakeShot("UploadingAttachment{0}".format(count))
            count += 1
        if photonum != 0 and count == photonum:
            raise Exception("Unable to see post success message")
        self.TakeShot("FinishComposePost")
        time.sleep(7 + photonum)  # bad: client needs a few secs to reload whole timeline from cloud

    def ViewFirstPost(self, photonum=0):
        self.TakeShot("BeforeViewFirstPost")
        # not sure if it works for different builds
        obj = self.app["Waveface"]["Create a New PostWindowsForms10.Window.8.app.0.360e0336"].WrapperObject()
        # trick: move cursor to target position then send click message
        obj.Children()[0].ClickInput(coords=(10, 10))
        obj.Children()[0].Click(coords=(10, 10))
        # client needs time to load photos
        for i in range(photonum + 1):
            self.TakeShot("ViewFirstPost{0}".format(i), 1)

    def AddComment(self, comment=''):
        self.app["Waveface"]["Comment"].Click()
        commentwin = self.app.top_window_()
        commentwin.TypeKeys(comment)
        self.TakeShot("TypeTextInComment")
        commentwin["Send"].Click()
        self.TakeShot("FinishAddComment")

    def PageUpTimeline(self, times=1):
        self.app["Waveface"]["Create a New PostWindowsForms10.Window.8.app.0.360e0336"].ClickInput()
        self.app["Waveface"]["Create a New PostWindowsForms10.Window.8.app.0.360e0336"].TypeKeys("{PGUP}")
        for i in range(times):
            self.TakeShot("PageUpTimeline{0}".format(i))

    def PageDownTimeline(self, times=1):
        self.app["Waveface"]["Create a New PostWindowsForms10.Window.8.app.0.360e0336"].ClickInput()
        self.app["Waveface"]["Create a New PostWindowsForms10.Window.8.app.0.360e0336"].TypeKeys("{PGDN}")
        for i in range(times):
            self.TakeShot("PageDownTimeline{0}".format(i))

    def PageUpExplorer(self, times=1):
        self.app["Waveface"]["Internet Explorer_Server"].ClickInput()
        self.app["Waveface"]["Internet Explorer_Server"].TypeKeys("{PGUP}")
        for i in range(times):
            self.TakeShot("PageUpExplorer{0}".format(i), 0)

    def PageDownExplorer(self, times=1):
        self.app["Waveface"]["Internet Explorer_Server"].ClickInput()
        self.app["Waveface"]["Internet Explorer_Server"].TypeKeys("{PGDN}")
        for i in range(times):
            self.TakeShot("PageDownExplorer{0}".format(i), 0)

    def ViewFirstPhoto(self):
        self.app["Waveface"]["WindowsForms10.Window.8.app.0.360e0337"].Click(coords=(10, 10))
        # bad: need to enter right arrow 3 times to focus on first photo
        self.app["Photo Viewer"].TypeKeys("{RIGHT}")
        self.app["Photo Viewer"].TypeKeys("{RIGHT}")
        self.app["Photo Viewer"].TypeKeys("{RIGHT}")
        self.TakeShot("ViewFirstPhoto")

    def ViewPrevPhoto(self):
        self.app["Photo Viewer"].TypeKeys("{LEFT}")
        self.TakeShot("ViewPrevPhoto")

    def ViewNextPhoto(self):
        self.app["Photo Viewer"].TypeKeys("{RIGHT}")
        self.TakeShot("ViewNextPhoto")

    def SaveAs(self, filepath):
        self.app["Photo Viewer"]["Button2"].Click()
        self.app["Save As"]["Edit"].TypeKeys(filepath)
        self.TakeShot("SaveAs")
        self.app["Save As"]["Save"].Click()
        self.TakeShot("ConfirmSaveAs")
        self.app.top_window_()["OK"].Click()
        self.app["Photo Viewer"].Close()

    def SaveAllToDesktop(self, folder):
        self.app["Photo Viewer"]["Button3"].Click()
        self.app["Browse For Folder"]["Make New Folder"].Click()
        self.app["Browse For Folder"].TypeKeys(folder)
        self.TakeShot("SaveAll")
        self.app["Browse For Folder"].TypeKeys("{ENTER}")
        self.app["Browse For Folder"]["OK"].Click()
        self.TakeShot("ConfirmSaveAll")
        self.app.top_window_()["OK"].Click()
        self.app["Photo Viewer"].Close()

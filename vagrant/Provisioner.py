# this script can only be interpreted by Python 3.2
from urllib import request
from html.parser import HTMLParser
import subprocess
import shlex
import winreg
import logging
import re

SHAWNLIANG_PC = 'http://192.168.1.197:9191'


def compare_version(ver1, ver2):
    if ver1 == '':
        return ver2
    elif ver2 == '':
        return ver1
    ver1_list = [int(x) for x in ver1.split('.')]
    ver2_list = [int(x) for x in ver2.split('.')]
    shortlen = min((len(ver1_list), len(ver2_list)))
    for i in range(shortlen):
        if ver1_list[i] > ver2_list[i]:
            return ver1
        elif ver1_list[i] < ver2_list[i]:
            return ver2
    return ver1 if len(ver1_list) >= len(ver2_list) else ver2


class MyHTMLParser(HTMLParser):
    maxver = ''

    def handle_starttag(self, tag, attrs):
        if tag == 'a':
            for k, v in attrs:
                if k == 'href':
                    result = re.match('/WavefaceSetup-(.+)\.exe', v)
                    if (result):
                        self.maxver = compare_version(self.maxver, result.group(1))


def main():
    logging.basicConfig(level=logging.INFO)
    try:
        with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, 'SOFTWARE\\Wammer\\WinStation') as key:
            logging.info("Waveface Station registry existed")
            value, regtype = winreg.QueryValueEx(key, 'stationId')
            logging.info("Waveface Station already installed")
    except WindowsError:
        logging.info("Waveface Station is not installed")
        try:
            parser = MyHTMLParser()
            data = request.urlopen(SHAWNLIANG_PC).read()
            parser.feed(str(data))
            logging.info("The latest version is {0}".format(parser.maxver))
            target_exe = 'WavefaceSetup-{0}.exe'.format(parser.maxver)
            target_url = '{0}/{1}'.format(SHAWNLIANG_PC, target_exe)
            logging.info('Downloading {0}'.format(target_url))
            request.urlretrieve(target_url, target_exe)
            subprocess.call(shlex.split(target_exe), shell=True)
        except Exception:
            logging.info("Unable to get/install WammerStation.msi, please check your network/environment")
            input('press any key to continue')


if __name__ == '__main__':
    main()

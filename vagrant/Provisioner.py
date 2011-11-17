# this script can only be interpreted by Python 3.2
from urllib import request
import subprocess
import shlex
import winreg
import logging


def main():
    logging.basicConfig(level=logging.INFO)
    try:
        with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, 'SOFTWARE\\Wammer'):
            logging.info("Waveface Station already installed")
    except WindowsError:
        logging.info("Waveface Station is not installed")
        try:
            request.urlretrieve('http://shawnliang-pc:9191/WammerStation.msi', 'WammerStation.msi')
            subprocess.call(shlex.split('WammerStation.msi'), shell=True)
        except Exception:
            logging.info("Unable to get/install WammerStation.msi, please check your network/environment")
            input('press any key to continue')


if __name__ == '__main__':
    main()

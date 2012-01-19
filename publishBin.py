import os
import sys
import glob
import shutil
from ftplib import FTP

solution_dir = os.path.dirname(os.path.abspath(__file__))
Wammer_release_dir = os.path.join(solution_dir, "Client/Wammer/bin/Release")
StationService_release_dir = os.path.join(solution_dir, "Station/Wammer.Station.Service/bin/Release")
StationSystemTray_release_dir = os.path.join(solution_dir, "Station/StationSystemTray/bin/Release")

if __name__ == "__main__":
    version = sys.argv[1]
    os.mkdir(version)
    for pdb in glob.glob("{0}/*.pdb".format(Wammer_release_dir)):
        shutil.copy2(pdb, version)
    for pdb in glob.glob("{0}/*.pdb".format(StationSystemTray_release_dir)):
        shutil.copy2(pdb, version)
    for pdb in glob.glob("{0}/*.pdb".format(StationService_release_dir)):
        shutil.copy2(pdb, version)
    shutil.copy2('{0}/development-WavefaceSetup-{1}.exe'.format(os.path.dirname(solution_dir), version), version)
    shutil.copy2('{0}/production-WavefaceSetup-{1}.exe'.format(os.path.dirname(solution_dir), version), version)
    shutil.copy2('{0}/staging-WavefaceSetup-{1}.exe'.format(os.path.dirname(solution_dir), version), version)
    ftp = FTP("WF-NAS", "admin", "waveface")
    ftp.mkd("Users/WavefaceStation/Builds/{0}".format(version))
    ftp.cwd("Users/WavefaceStation/Builds/{0}".format(version))
    os.chdir(version)
    for filename in glob.glob("*".format(version)):
        print filename
        with open(filename, 'rb') as fp:
            ftp.storbinary("STOR {0}".format(filename), fp)
    ftp.close()
    os.chdir("..")
    shutil.rmtree(version)

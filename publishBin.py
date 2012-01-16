import os
import sys
import glob
import shutil
import logging,socket,re
import subprocess

solution_dir = os.path.dirname(os.path.abspath(__file__))
Wammer_release_dir = os.path.join(solution_dir, "Client\\Wammer\\bin\\Release")
StationSetup_release_dir = os.path.join(solution_dir, "Client/StationSetup/bin/Release")
StationService_release_dir = os.path.join(solution_dir, "Station/Wammer.Station.Service/bin/Release")

def mountNetworkDriveForWin(strDisk,strUncPath,strUserName,strPassword='',readOnly=0, persistent=0):
    '''
    Mount a remote folder on local drive
    @param strDisk: specify a local disk name. Ex. "M:" or "*" to mount on any empty device.
    @param strUncPath: specify the remote folder. Ex. r"\\10.2.203.200\cpm"
    @param strUserName: specify login name
    @param strPassword: specify login password
    @param readOnly: reserved
    @param persistent: persistent or not, default is False
    @return: 0 means success; otherwise return 2.
    '''
    if not strPassword: strPassword='""'
    strCmd = 'net use %s %s %s'%(strDisk,strUncPath,strPassword)
    if strUserName: strCmd+=" /u:%s"%strUserName
    if persistent:  strCmd+=" /PERSISTENT:YES"
    objProcess=subprocess.Popen(strCmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    strStdout=objProcess.stdout.read()
    intRc = objProcess.wait()
    if intRc and (not strStdout or strStdout.find(' 85')<0):
        logging.error("mount %s on %s failed. ErrorCode: %s stdout:\n%s",strUncPath,strDisk,intRc,strStdout)
        return 2
    if strDisk=='*':
        strDisk=findDeviceName(strUncPath)
        if not strDisk:
            logging.error('mount %s on "*" failed.',strUncPath)
            return 2
        strStdout=os.popen('net use').read()
    else:
        strStdout=os.popen('net use %s'%strDisk).read()
    if not re.search(re.escape(strUncPath), strStdout, re.I):
        logging.error('mount %s on %s failed. %s already mounted:\n%s',strUncPath,strDisk,strDisk,strStdout.strip())
        return 2
    try:
        os.listdir(strDisk+"/")
    except:
        logging.error("mount %s on %s failed. %s",strUncPath,strDisk,sys.exc_info()[1])
        logging.error('mount command: "%s"',strCmd)
        unmountNetworkDrive(strDisk)
        return 2
    logging.info("mount %s on %s successfully.",strUncPath,strDisk)
    return 0

if __name__ == "__main__":
    version = sys.argv[1]
    os.mkdir(version)
    for pdb in glob.glob("{0}/*.pdb".format(Wammer_release_dir)):
        shutil.copy2(pdb, version)
    for pdb in glob.glob(r"{0}/*.pdb".format(StationSetup_release_dir)):
        shutil.copy2(pdb, version)
    for pdb in glob.glob(r"{0}/*.pdb".format(StationService_release_dir)):
        shutil.copy2(pdb, version)
    shutil.copy2('{0}\\development-WavefaceSetup-{1}.exe'.format(os.path.dirname(solution_dir), version), version)
    shutil.copy2('{0}\\production-WavefaceSetup-{1}.exe'.format(os.path.dirname(solution_dir), version), version)
    mountNetworkDriveForWin('Z:', r"\\WF-NAS\Users", 'admin', strPassword='waveface', readOnly=0, persistent=1)
    shutil.move(version, r"Z:\WavefaceStation\Builds")

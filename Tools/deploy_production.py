import os
import sys
import s3_upload
import glob
import re
import subprocess

def find_aostream_bin(aostream_dir):
	bins = glob.glob(os.path.join(aostream_dir, "production-*.exe"))

	if len(bins) == 0:
		raise EnvironmentError("aostream binary is not found in {0}".format(aostream_dir))

	if len(bins) > 1:
		raise EnvironmentError("Too many production-*.exe in {0}".format(aostream_dir))

	return bins[0]


def get_version_from_binary(binary_file):
	match = re.search(r"(\d\.\d\.\d\.\d+)", binary_file)
	if (match):
		return match.group(1)
	else:
		raise EnvironmentError("Is {0} a correct aostream binary??".format(binary_file))


def generate_versioninfo(version, aostream_dir):
	ret = subprocess.call(["python", "versioninfo.py", version, aostream_dir])
	if ret != 0:
		raise RuntimeError("generate version info error: {0}".format(ret))


def upload_versioninfo(aostream_dir, version):
	appcast_file = os.path.join(aostream_dir, "versioninfo.xml")
	
	ret = subprocess.call([
		r"C:\Program Files (x86)\WinSCP\WinSCP.exe", 
		"/console",
		"/command",
		"open wammer@waveface.com",
		"option confirm off",
		"put {0} ./static/extensions/windowsUpdate/versioninfo.xml".format(appcast_file),
		"close",
		"exit"])


	if ret != 0:
		raise RuntimeError("unable to upload {0}: {1}".format(appcast_file, ret))


if __name__ == "__main__":

	aostream_dir = sys.argv[1]

	binfile = find_aostream_bin(aostream_dir)
	version = get_version_from_binary(binfile)

	generate_versioninfo(version, aostream_dir)
	
	upload_versioninfo(aostream_dir, version)

	s3_upload.upload_s3(binfile, "aostream-win-{0}.exe".format(version))

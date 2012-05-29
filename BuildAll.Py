import os
import sys
import re
import buildVersion
import packageVersion
import changeLogLevel
import shutil
import os.path
import wfVersion

solution_dir = sys.argv[1]
build_num = sys.argv[2]



def build_target(solution_dir, target, version, build_num) :
	# target can be "production", "development", or "staging"
	
	if target == "staging" or target == "production":
		target_cfg_file = os.path.join(solution_dir, r"Setup\InstallHelper\app.config.{0}.xml".format(target))
		cfg_file = os.path.join(solution_dir, r"Setup\InstallHelper\app.config")
		shutil.copy(target_cfg_file, cfg_file)
	
	ret = os.system(r"msbuild.exe /t:Rebuild /p:Configuration=Release")
	
	if ret != 0:
		raise Exception(r"msbuild.exe returned failure: {0}".format(ret))
		
	if target == "production" or target == "staging":
		pfx_path = os.path.join(solution_dir, "WavefaceCert.pfx")
		signee = os.path.join(solution_dir, r"Setup\Release\WavefaceSetup.exe")
		ret = os.system(r'signtool.exe sign /f "{0}" /p waveface /t "http://timestamp.verisign.com/scripts/timstamp.dll" /v "{1}"'.format(pfx_path, signee))
		if ret != 0:
			raise Exception("signtool returned failure: {0}".format(ret))
	
	built_path = os.path.join(solution_dir, r"Setup\Release\WavefaceSetup.exe")
	publish_path = os.path.join(solution_dir, r"..\{0}-WavefaceSetup-{1}.{2}.exe".format(target, version, build_num))
	shutil.copy(built_path, publish_path)


if __name__ == "__main__":
	# get version string : major.minor.patch
	version = wfVersion.get_product_version_from_wix_config_file(solution_dir)
	version_full = version + "." + build_num
	print "==== Building Waveface Stream " + version_full + " ===="

	# change version strings in all AssemblyInfo.cs
	buildVersion.dir_traverse(solution_dir, version_full)

	# change build number in wix config file
	packageVersion.find_and_replace(solution_dir + r'\Setup\Package\Product.wxs', build_num)

	# change log level to "INFO"
	changeLogLevel.dir_traverse(solution_dir, version_full)

	build_target(solution_dir, "development", version, build_num)
	build_target(solution_dir, "staging", version, build_num)
	build_target(solution_dir, "production", version, build_num)

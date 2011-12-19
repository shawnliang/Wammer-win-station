import os
import sys
import re
from tempfile import mkstemp
from shutil import move
from os import remove, close

def find_and_replace(target, version):
    version = '"{0}"'.format(version)
    pattern = re.compile('"1\.0\.0\.0"')
    
    fh, abs_path = mkstemp()
    new_file = open(abs_path, 'w')
    old_file = open(target)
    
    for line in old_file:
        new_file.write(pattern.sub(version, line))
    new_file.close()
    close(fh)
    old_file.close()
    remove(target)
    move(abs_path, target)

def dir_traverse(dest, version):
    for dirname, dirnames, filenames in os.walk(dest):
        for filename in filenames:
            if (filename == 'AssemblyInfo.cs'):
                target = os.path.join(dirname, filename)
                find_and_replace(target, version)
        for subdir in dirnames:
            dir_traverse(os.path.join(dirname, subdir), version)

print "[Waveface] Replace version from {0} to {1}".format(sys.argv[1], sys.argv[2])
dir_traverse(sys.argv[1], sys.argv[2])
print "[Waveface] Version replacement done."

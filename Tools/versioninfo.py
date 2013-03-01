
import sys
import os
import datetime

solution_dir = sys.argv[2]


def genxml(link, title, rnote, pubdate, binfile, binurl, binlen, binver, xmlfile):
    xml = """<?xml version="1.0" encoding="utf-8"?>

<rss version="2.0" xmlns:sparkle="http://www.andymatuschak.org/xml-namespaces/sparkle"  xmlns:dc="http://purl.org/dc/elements/1.1/">
    <channel>
        <title>aostream station for Windows</title>
        <link>{0}</link>
        <description></description>
        <language>all</language>
        <item>
            <title>{1}</title>
            <sparkle:releaseNotesLink>{2}</sparkle:releaseNotesLink>
            <pubDate>{3}</pubDate>
            <enclosure
                url="{4}"
                length="{5}"
                type="application/octet-stream"
                sparkle:version="{6}"
            />
        </item>
    </channel>
</rss>
""".format(link, title, rnote, pubdate, binurl, binlen, binver)

    with open(os.path.join(solution_dir, xmlfile), "w") as f:
        f.write(xml)


if __name__ == "__main__":
    print "Generate versioninfo.xml"
    version = sys.argv[1]
    pubdate = datetime.datetime.utcnow().replace(microsecond=0).isoformat(' ') + '+0800'

    # development
    dev_link = "http://develop.waveface.com:4343/extensions/windowsUpdate/versioninfo.xml"
    dev_title = "Version {0}".format(version)
    dev_rnote = "http://develop.waveface.com:4343/extensions/windowsUpdate/rnotes.html"
    dev_binfile = "development-WavefaceSetup-{0}.exe".format(version)
    dev_binurl = "http://develop.waveface.com:4343/extensions/windowsUpdate/{0}".format(dev_binfile)
    dev_binlen = os.path.getsize(os.path.join(solution_dir, dev_binfile))
    dev_binver = version
    genxml(dev_link, dev_title, dev_rnote, pubdate, dev_binfile, dev_binurl, dev_binlen, dev_binver, "versioninfo_dev.xml")

    # production
    link = "https://waveface.com/extensions/windowsUpdate/versioninfo.xml"
    title = "Version {0}".format(version)
    rnote = "https://waveface.com/release/windows.en.html"
    binfile = "production-WavefaceSetup-{0}.exe".format(version)
    binurl = "https://waveface.com/extensions/windowsUpdate/{0}".format(binfile)
    binlen = os.path.getsize(os.path.join(solution_dir, binfile))
    binver = version
    genxml(link, title, rnote, pubdate, binfile, binurl, binlen, binver, "versioninfo.xml")


import sys
import os
import datetime

solution_dir = os.path.dirname(os.path.abspath(__file__))


def genxml(link, title, rnote, pubdate, binfile, binurl, binlen, binver, xmlfile):
    xml = """<?xml version="1.0" encoding="utf-8"?>

<rss version="2.0" xmlns:sparkle="http://www.andymatuschak.org/xml-namespaces/sparkle"  xmlns:dc="http://purl.org/dc/elements/1.1/">
    <channel>
        <title>Waveface Windows Program</title>
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

    with open(os.path.join(os.path.dirname(solution_dir), xmlfile), "w") as f:
        f.write(xml)


if __name__ == "__main__":
    print "Generate versioninfo.xml"
    version = sys.argv[1]

    # development
    dev_link = "http://develop.waveface.com:4343/extensions/windowsUpdate/versioninfo.xml"
    dev_title = "Version {0}".format(version)
    dev_rnote = "http://develop.waveface.com:4343/extensions/windowsUpdate/rnotes-{0}.html".format(version)
    dev_pubdate = datetime.datetime.utcnow().replace(microsecond=0).isoformat(' ') + '+0800'
    dev_binfile = "development-WavefaceSetup-{0}.exe".format(version)
    dev_binurl = "http://develop.waveface.com:4343/extensions/windowsUpdate/{0}".format(dev_binfile)
    dev_binlen = os.path.getsize(os.path.join(os.path.dirname(solution_dir), dev_binfile))
    dev_binver = version
    genxml(dev_link, dev_title, dev_rnote, dev_pubdate, dev_binfile, dev_binurl, dev_binlen, dev_binver, "versioninfo_dev.xml")

    # production
    link = "https://waveface.com/extensions/windowsUpdate/versioninfo.xml"
    title = "Version {0}".format(version)
    rnote = "https://waveface.com/extensions/windowsUpdate/rnotes-{0}.html".format(version)
    pubdate = datetime.datetime.utcnow().replace(microsecond=0).isoformat(' ') + '+0800'
    binfile = "production-WavefaceSetup-{0}.exe".format(version)
    binurl = "https://waveface.com/extensions/windowsUpdate/{0}".format(binfile)
    binlen = os.path.getsize(os.path.join(os.path.dirname(solution_dir), binfile))
    binver = version
    genxml(link, title, rnote, pubdate, binfile, binurl, binlen, binver, "versioninfo.xml")

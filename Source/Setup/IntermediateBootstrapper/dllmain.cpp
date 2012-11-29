#include "stdafx.h"
#include <Bootstrapper.h>
#include <Prerequisites.h>

using namespace SharpSetup;
using namespace std;

class SetupBootstrapper : public BootstrapperBase
{
public:
	virtual DWORD OnUnpackingComplete()
	{
		PrerequisiteManager pm(*this);
		if(!isInstalledMsi(L"3.1"))
			installMsi(&pm, L"3.1");
		if(!isInstalledDotNet(L"3.5"))
			installDotNet(&pm, L"3.5");

		pm.getFiles();
		pm.performInstall();

		return S_OK;
	}
};
CREATEBOOTSTRAPPER(SetupBootstrapper);

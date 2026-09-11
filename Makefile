# HomemadeMXDeckControls - Developer Makefile
.PHONY: help status list link unlink logs restart dev-mode-on dev-mode-off profiles

help:
	@./bin/mxdev help

status:
	@./bin/mxdev status

list:
	@./bin/mxdev list

link:
	@./bin/mxdev link plugins/mx-custom-starter

unlink:
	@./bin/mxdev unlink mx-custom-starter

logs:
	@./bin/mxdev logs

restart:
	@./bin/mxdev restart

dev-mode-on:
	@./bin/mxdev dev-mode on

dev-mode-off:
	@./bin/mxdev dev-mode off

profiles:
	@python3 tools/profile_tool.py list

safari-build:
	@export NUGET_PACKAGES="$$PWD/.nuget"; export DOTNET_CLI_HOME="$$PWD/.dotnet_home"; ./bin/dotnet build plugins/safari-controller/SafariPlugin.csproj
	@python3 tools/generate_safari_profiles.py

safari-link: safari-build
	@./bin/mxdev link plugins/safari-controller/bin/Debug

safari-unlink:
	@./bin/mxdev unlink Debug

if not DEFINED IS_MINIMIZED set IS_MINIMIZED=1 && start "" /min "%~dpnx0" %* && exit

echo on

cd /D Z:\
cd luminaryserver
python wallclient.py r

exit



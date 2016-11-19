#Using Git

##Copying Repository
1. Open terminal or command prompt
2. Browse to the directory you want to use:
	* `dir` (`ls` for Mac/UNIX) - list the files and folders in the current directory
	* `cd` - change directory

		For example, `cd theGame` will browse into a folder called theGame.

		You can also go "up" a directory with `cd ..`
	* `pwd` - print working directory (tells you where you are)
3. Once you are in the directory, type the following commands:
```
git init
git remote add origin git@github.com:cgumb/timetravelmelee.git`
git pull
```
(skip this if you are working with a preexisting local copy)

4. Congratulations, you've copied the repository!

##Making Changes

1. get back into terminal in the folder with the project repository
2. type the following commands:
```
git add --all
git commit -m "<insert your notes about what changes you made here, keep it short>"
git push origin master
```
3. If everything went well, the rest of the group should be able to see your updates online!

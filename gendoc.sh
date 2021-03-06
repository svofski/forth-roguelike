mkdir -p html
gforth doc/gendoc.fs
echo "If the result of gendoc does not look nice, press ^C"
read wtf
for f in *.fs ; do
  mv $f.html html/
done
echo "If you don't want to update docs in git, press ^C"
read wtf
git stash
git checkout gh-pages
cp -f html/* .
git add *.html
git commit -m gendoc
git checkout master
git stash pop

mkdir -p html
gforth doc/gendoc.fs
for f in *.fs ; do
  mv $f.html html/
done
git stash
git checkout gh-pages
cp -f html/* .
git add *.html
git commit -m gendoc
git checkout master
git stash pop

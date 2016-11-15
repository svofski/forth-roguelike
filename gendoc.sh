mkdir -p html
gforth doc/gendoc.fs
for f in *.fs ; do
  mv $f.html html/
done

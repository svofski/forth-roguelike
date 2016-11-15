mkdir -p docs
gforth doc/gendoc.fs
for f in *.fs ; do
  mv $f.html docs/
done

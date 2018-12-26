12 constant NSCROLLS
40 constant NSYLLABLES

here
	," blech "
	," foo "
	," barf "
	," rech "
	," bar "
	," blech "
	," quo "
	," bloto "
	," woh "
	," caca "
	," blorp "
	," erp "
	," festr "
	," rot "
	," slie "
	," snorf "
	," iky "
	," yuky "
	," ooze "
	," ah "
	," bahl "
	," zep "
	," druhl "
	," flem "
	," behil "
	," arek "
	," mep "
	," zihr "
	," grit "
	," kona "
	," kini "
	," ichi "
	," niah "
	," ogr "
	," ooh "
	," ighr "
	," coph "
	," swerr "
	," mihln "
	," poxi "
NSYLLABLES csarray scroll-syllable

\ array of scroll real names
here
    ," of protect armor "    
    ," of hold monster "     
    ," of enchant weapon "   
    ," of enchant armor "    
    ," of identify "         
    ," of teleportation "    
    ," of sleep "            
    ," of scare monster "    
    ," of remove curse "     
    ," of create monster "   
    ," of aggravate monster "
    ," of magic mapping "    
NSCROLLS csarray scroll-real

constarray scroll-value
    505 ,
    200 ,
    235 ,
    235 ,
    175 ,
    190 ,
     25 ,
    610 ,
    210 ,
    100 ,
     25 ,
    180 ,

char-array scroll-status NSCROLLS allot

\ create buffers for scroll names
NSCROLLS mk-names (scroll-name)

\ get nth scroll called name as string
: scroll-name ['] (scroll-name) its-name ; 

: strcat ( buf str len -- )
    2 pick      ( buf str len buf -- )
    dup c@ +    ( buf str len buf-end -- )
        \ update buffer length
        over 4 pick dup -rot ( ... buf len buf -- )
        c@ + swap c!
    1+ swap move   ( buf -- ) \ copy str to buf+oldlen+1
    drop ; 

\ take pointer to c-string buffer and generate a scroll name
: bergh ( buf -- )
    1 6 any-between 0 do
        dup
        -1 NSYLLABLES any-between scroll-syllable ( buf buf syllable len -- )
        strcat 
    loop
    drop ;

: gen-scroll-names
    NSCROLLS 0 do
        0 i (scroll-name) c!
        i (scroll-name) bergh
    loop ;

\ n call-scroll
: call-scroll
    dup 
        (scroll-name) call-it 
        scroll-status CALLED swap c! ;

\ s" bollocks" 0 (scroll-name) place
\ 0 scroll-name type

: type-scroll-info    
    dup ." $: " scroll-value .
    dup ."  status: " scroll-status c@ .
    dup ."  name: " scroll-name type
    dup ."  real: " scroll-real type
    drop ;

: dump-scrolls
    cr
    NSCROLLS 0 do
        i dup . type-scroll-info cr
    loop ;


\ gen-scroll-names
\ type-scroll-names
\ 3 call-scroll



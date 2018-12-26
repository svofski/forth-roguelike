decimal 

14 constant NPOTIONS

\ array of potion real names
here
    ," of increase strength "
    ," of restore strength "
    ," of healing "
    ," of extra healing "
    ," of poison "
    ," of raise level "
    ," of blindness "
    ," of hallucination "
    ," of detect monster "
    ," of detect things "
    ," of confusion "
    ," of levitation "
    ," of haste self "
    ," of see invisible "
NPOTIONS csarray potion-real

constarray potion-value
    100 , 
    250 ,
    100 ,
    200 ,
    10 ,
    300 ,
    10 ,
    25 ,
    100 ,
    100 ,
    10 ,
    80 ,
    150 ,
    145 ,

char-array potion-status NPOTIONS allot

\ create buffers for potion names (colors)
NPOTIONS mk-names (potion-name)

here
    ," blue "
    ," red "
    ," green "
    ," grey "
    ," brown "
    ," clear "
    ," pink "
    ," white "
    ," purple "
    ," black "
    ," yellow "
    ," plaid "
    ," burgundy "
    ," beige "
NPOTIONS csarray colors

: gen-potion-names
    NPOTIONS mkshuffle
    NPOTIONS 0 do
        colors
        i (potion-name) place
    loop ;

\ get nth potion called name as string
: potion-name ['] (potion-name) its-name ;

\ n call-potion
: call-potion
    dup 
        (potion-name) call-it 
        potion-status CALLED swap c! ;

: type-potion-info    
    dup ." $: " potion-value .
    dup ."  status: " potion-status c@ .
    dup ."  name: " potion-name type
    dup ."  real: " potion-real type
    drop ;

: dump-potions
    cr
    NPOTIONS 0 do
        i dup . type-potion-info cr
    loop ;


\ gen-potion-names
\ dump-potions


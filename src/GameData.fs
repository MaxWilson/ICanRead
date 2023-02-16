module GameData

let words = 
    let raw = """
a able about above absolute accent after again am and are as ashamed aside ask asleep assist associate assume assure at
background bacon bad bag bake balance ball banana band bang bank bar bare be bed beyond big bike bill billion bin bind bird birth biscuit bit black blow blue board boat body boil bomb bond bone book boy branch brand brave bread break breakfast breast breath breathe breed brick brown bush business busy but butter button buy by
cable cage cake calculate call calm camera can cat chain chew chicken chief child chip chocolate choice choose chop christmas church cigarette coast come community company compare competition complain complete could curl current customer cut
dad damage dance danger desk did directed direction dirty disappear disappoint discipline discover discuss disease disgust dish distance district disturb dive divide divorce do doctor dog doll dollar door double doubt down dump during dust duty
each ear early earn earth ease east easy eight eleven else email embarrass emotion empire employ empty encourage end enemy energy engage engine engineer enjoy enormous enough enter entertain entire envelope environment equal equipment escape especially establish estate
feet first five flame flash flat flight flip float flood floor flow flower fly fold folk follow food fool foot football for force foreign forest forget forgive form forth fortnight fortunate fortune forward four fox frame frankly free freeze fresh friday friend fright frog from front frost fruit frustrate fry full fun fund fur furniture further future
gain game garage get girl give glad glance glass glory go going good green group grow guarantee guard guess guest guide guilty gun guy
had have he heap hear heart heat heaven heavy hedge height hell hello help her here hero hesitate hide high hill hire history hit hobby hold hole holiday home honest honey honour hook hope horse house
i ice idea identify idiot if ignore ill illustrate image imagine immediate important impress improve in inch include income increase incredible indeed indicate individual industry influence inform injure innocent inside insist into is
know
lettuce library licence lid lie life lift light like little look
make man math me mother mrs much mud mum murder muscle music must my mystery
nail name nanny nest never new news newspaper next nice night nine no nobody noise non none nor normal north northern nose not note nothing notice november now nowhere number nurse nut
oak object of on one out over
panic paper pardon parent park part particular partner party pass past pat patch path patient pattern pause pay peace pen penny pension play pleasant please pleasure plenty plug plus pocket poem poet point poison pole police pride prime prince print prison privacy private pump punch punish pup purchase pure purple purpose push put
qualify quality quarter queen question quick quiet quit quite quote
rabbit race radio rain raise range rapid red ring rip rise risk river road roar rob rock role run
sad safe said saw school see seven she shoot shop shore short should shoulder shout shove show shower shut shy sick side sight sign signal silence silly silver similar simple since six smile smoke smooth snake snap snow so social society species specific speech speed spell spend stop store
the this three to tone tongue tonight too travel tray treat tree trial two type typical
under
walk was when who why will wind window wine wing winter wipe wire wise wish with within without witness wolf woman
yes you
zero"""
    raw.Trim().Split(' ', '\r', '\n', ',', ';') |> Array.filter (not << System.String.IsNullOrWhiteSpace) |> Array.map (fun x -> x.ToLowerInvariant()) |> Array.distinct
    // I used this code to help create the above list of words by processing text.    
    //|> Array.sort
    //|> Array.groupBy (fun w -> w[0])
    //|> Array.map (fun (_, words) -> System.String.Join(" ", words))
    //|> fun x -> System.String.Join("\n", x) |>  TextCopy.Clipboard().SetText
// #r "nuget: TextCopy"

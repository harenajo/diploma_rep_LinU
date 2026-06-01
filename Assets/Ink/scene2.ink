VAR prologue_timer = false
VAR class_route = false
VAR ariya_points = 0
VAR lev_points = 0
VAR roman_points = 0
VAR markian_points = 0
VAR study_points = 0
VAR failed_cipher_attempts = 0
VAR fear = 0
VAR told_lev_about_message = false
VAR told_lev_about_second_mail = false
VAR told_ariya_truth = false
VAR hinted_ariya = false
VAR hid_truth_from_ariya = false
VAR asked_ariya_about_book = false
VAR found_aiia_hint = false
VAR solved_cipher_2 = false
VAR got_second_mail = false
VAR walked_with_ariya = false
VAR met_lev_on_walk = false
VAR checked_bookshelf = false
VAR checked_desk = false
VAR checked_bed = false
VAR opened_locked_file = false
VAR made_caesar_name_mistake = false
VAR found_caesar_key = false
VAR caesar_mistakes = 0
VAR file_opened = false

=== start ===
# bg: morning
# clear: all
# textbox: black

Наступний день. Ранок.

# show: Lev,right,fullbody

# speaker: Lev
# emotion: fullbody
# textbox: blue
Ну ти йдеш вже?

# speaker: Mark
# textbox: red
Ну зараз ще 5 хвилин. Зараз зуби дочищу і йдем.

# speaker: Mark
# textbox: red
Бачив, Арія в чаті написала, що сьогодні третю пару перенесуть? То може я не піду на практичну, а одразу до Романа зайду.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Бачив. Давай швидше, бо автобус пропустимо.

-> classroommorning


=== classroommorning ===
# bg: hallway
# clear: all
# textbox: black

10 хвилин перед початком пари.

В коридорі чути гомін інших студентів.

# show: Mark,left,fullbody
# show: Lev,right,fullbody

# speaker: Lev
# emotion: fullbody
# textbox: blue
А я тобі казав.

# speaker: Mark
# emotion: fullbody
# textbox: red
Ну ладно тобі. Нормально все. Встигли.

# speaker: none
# textbox: pink
Привіт, хлопці!

# textbox: black
Пролунав дівочий голос позаду них. Вони обернулись.

Арія.

Староста їхньої групи. Вона посміхалась, махаючи їм рукою.

# show: Ariya,right,fullbody

# speaker: Lev
# emotion: fullbody
# textbox: blue
Хей.

# speaker: Mark
# emotion: fullbody
# textbox: red
Привіт, Аріє.

# speaker: Ariya
# emotion: fullbody
# textbox: pink
Які люди! Давно не бачились. Якраз вчасно. Заходьте.

-> classroom


=== classroom ===
# bg: classroom1
# clear: all
# show: Mark,left,fullbody
# show: Ariya,center,fullbody
# show: Lev,right,fullbody

# clear: all
# show: Mark,left,smile
# show: Ariya,center,thinking

# speaker: Mark
# emotion: smile
# textbox: red
Як пройшли твої вихідні?

# speaker: Ariya
# emotion: thinking
# textbox: pink
Якщо чесно, нічого цікавого. Сиділа вдома, робила лабораторні... Нудьга одним словом. А ви?

# speaker: Mark
# emotion: other2
# textbox: red
Так само...

# speaker: Ariya
# emotion: neutral
# textbox: pink
Ти це так сказав... неправдоподібно.

+ [Сказати, що просто не виспався]
    ~ hid_truth_from_ariya = true
    -> ariya_tired

+ [Натякнути, що день був дивний]
    ~ ariya_points += 1
    ~ hinted_ariya = true
    -> ariya_hint


=== ariya_tired ===
# speaker: Mark
# emotion: other2
# textbox: red
Та просто не виспався. Всю ніч сидів за ноутом.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Тоді тримайся. До кінця дня ще далеко.

-> class_time


=== ariya_hint ===
# speaker: Mark
# emotion: thinking
# textbox: red
Скажімо так... вчора був дивний вечір.

# speaker: Ariya
# emotion: thinking
# textbox: pink
Дивний — це наскільки?

# speaker: Mark
# emotion: other2
# textbox: red
Дивний. Але все добре.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Гаразд. Але якщо буде треба — я вмію слухати.

-> class_time


=== class_time ===
# clear: all
# bg: classroom1
# textbox: black

Почалась пара. Ніхто особливо нічого і не робив. Хто в телефоні грав, хто в ноутбуці лабораторні з іншого предмету робив.

Двома словами — звичайна середа.

Минула перша лекція, за нею і друга.

Під кінець другої пари екран ноутбука коротко блимнув.

# sfx: notification
На робочому столі з'явилось нове повідомлення від невідомого відправника.

# mail_icon: new
# add_mail: 2

# show: Mark,left,thinking

# speaker: Mark
# emotion: other2
# textbox: red
Знову...

# textbox: black
Марк потягнувся до тачпада, але викладач саме оголосив кінець пари. Аудиторія одразу наповнилась шумом, стільці заскрипіли, хтось почав збирати речі.

# speaker: Mark
# emotion: thinking
# textbox: red
Не тут. В кімнаті подивлюсь.

~ got_second_mail = true

-> after_classes


=== after_classes ===
# bg: hallway
# clear: all
# show: Mark,left,neutral
# show: Ariya,right,neutral
# textbox: black

Пара закінчилась. Студенти посипались з кабінетів у коридори.

Лев, як завжди, пішов за матча лате в кав’ярню неподалік.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Що збираєшся робити зараз?

# speaker: Mark
# emotion: neutral
# textbox: red
Піду до пана Романа. Хотів дещо дізнатися...

# speaker: Ariya
# emotion: thinking
# textbox: pink
За курсову?

+ [Розказати правду про повідомлення]
    ~ ariya_points += 2
    ~ told_ariya_truth = true
    -> tell_ariya_truth_before_roman

+ [Сказати, що за курсову]
    ~ hid_truth_from_ariya = true
    -> lie_about_coursework_before_roman

+ {hinted_ariya} [Сказати тільки частину правди]
    ~ ariya_points += 1
    -> half_truth_before_roman


=== tell_ariya_truth_before_roman ===
# speaker: Mark
# emotion: other
# textbox: red
Мені почали приходити дивні листи.

# speaker: Mark
# emotion: other2
# textbox: red
Вчора було повідомлення з дивним набором букв, а потім вони надіслали Леву моє фото, яке він не мав бачити.

# speaker: Ariya
# emotion: thinking
# textbox: pink
Це дивно

# speaker: Mark
# emotion: neutral
# textbox: red
Знаю. Тому і хочу поговорити з Романом.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Тоді я почекаю в коридорі. Після розмови можемо пройтись, якщо хочеш, удачі.

-> Classroomteacher


=== lie_about_coursework_before_roman ===
# speaker: Mark
# emotion: neutral
# textbox: red
Так, за курсову. Треба літературу уточнити.

# speaker: Ariya
# emotion: thinking
# textbox: pink
Зрозуміло. Тоді не буду заважати.

# speaker: Ariya
# emotion: smile
# textbox: pink
Але після цього можеш пройтись зі мною. День занадто хороший, щоб одразу ховатися в гуртожитку.

-> Classroomteacher


=== half_truth_before_roman ===
# speaker: Mark
# emotion: thinking
# textbox: red
Мене просто зацікавили шифри. І це трохи пов'язано з тим дивним вечором, про який я казав.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Тобто не курсова?

# speaker: Mark
# emotion: other2
# textbox: red
Не зовсім. Частково

-> Classroomteacher


=== Classroomteacher ===
# bg: hallway
# clear: all
# show: Mark,left,fullbody
# show: Ariya,right,fullbody

# speaker: Ariya
# emotion: fullbody
# textbox: pink
Ну добре, тоді до зустрічі.

# hide: Ariya

# textbox: black
Вона помахала йому рукою та пішла.

Марк пройшов по коридору прямо до потрібного кабінету. За дверима доносилась тиха розмова. Він постукав у двері та зайшов.

# bg: classroom2
# clear: all
# show: Mark,left,fullbody
# show: Roman,romanClose,fullbody
# show: Markian,markianClose,fullbody

# speaker: Mark
# emotion: fullbody
# textbox: red
Добрий день.

# textbox: black
У кабінеті було двоє людей. Пан Роман стояв біля столу з відкритим ноутбуком, а Маркіян Захарович стояв поруч, неквапливо попиваючи каву з чашки.

# speaker: Markian
# emotion: fullbody
# textbox: yellow
Добрий.

# speaker: Markian
# emotion: fullbody
# textbox: yellow
Ну, робота рухається непогано. Гляньте ще додаткову літературу по цьому питанню. Можливо, я щось підкину пізніше.

# speaker: Roman
# emotion: fullbody
# textbox: violet
Добре, я перегляну. Там є кілька джерел, але вони суперечать одне одному.

# speaker: Markian
# emotion: fullbody
# textbox: yellow
У науковій роботі вони майже завжди суперечать одне одному. Питання в тому, хто переконливіший.

# speaker: Markian
# emotion: fullbody
# textbox: yellow
Не буду відволікати вас, Романе. Гарного дня.

# speaker: Mark
# emotion: fullbody
# textbox: red
До побачення.

# speaker: Markian
# emotion: fullbody
# textbox: yellow
До побачення, Марку.

# hide: Markian
# show: Roman,right,fullbody
# show: Mark,left,fullbody

# textbox: black
Двері зачинились за Маркіяном Захаровичем. У кабінеті стало тихіше.

# speaker: Roman
# emotion: fullbody
# textbox: violet
Слухаю.

# speaker: Mark
# emotion: other
# textbox: red
Останнім часом мене зацікавили шифри.

# speaker: Roman
# emotion: thinking
# textbox: violet
Шифри? О як?

# speaker: Roman
# emotion: neutral
# textbox: violet
І чому ви прийшли до мене?

# speaker: Mark
# emotion: other
# textbox: red
Я чув що ви знаєтесь на цій темі дуже добре і хотів запитатися вашої поради.

+ [Сказати, що це для курсової]
    -> roman_coursework

+ [Сказати, що це особистий інтерес]
    ~ roman_points += 1
    -> roman_interest

+ [Запитати, хто ще цікавився ними]
    ~ roman_points += 1
    -> roman_old_students


=== roman_coursework ===
# speaker: Mark
# emotion: neutral
# textbox: red
Насправді це для курсової.

# speaker: Roman
# emotion: thinking
# textbox: violet
Курсова — зручна причина ставити правильні питання.

-> roman_books


=== roman_interest ===
# speaker: Mark
# emotion: thinking
# textbox: red
Це не тільки для курсової. Мені справді стало цікаво.

# speaker: Roman
# emotion: neutral
# textbox: violet
Справжній інтерес — небезпечна річ. Через нього люди знаходять більше, ніж планували.

# speaker: Mark
# emotion: other2
# textbox: red
Це має звучати як попередження?

# speaker: Roman
# emotion: smile
# textbox: violet
Як спостереження.

-> roman_books


=== roman_old_students ===
# speaker: Mark
# emotion: thinking
# textbox: red
А хто ще цікавився такими речами? Ну, в університеті.

# speaker: Roman
# emotion: neutral
# textbox: violet
Були люди.

# speaker: Roman
# emotion: neutral
# textbox: violet
Студенти, які любили загадки, старі архіви, шифри.

# speaker: Mark
# emotion: other
# textbox: red
І що з ними сталося?

# speaker: Roman
# emotion: neutral
# textbox: violet
...

# speaker: Roman
# emotion: neutral
# textbox: violet
Нічого

-> roman_books


=== roman_books ===
# speaker: Roman
# emotion: smile
# textbox: violet
Почніть із класичних шифрів. Можу порекомендувати пару книг для початку.

# textbox: black
Роман відірвав листок і написав кілька назв. Він передав його Марку.

# speaker: Roman
# emotion: neutral
# textbox: violet
Мені вже час іти, у мене ще справи на кафедрі. Над курсовою працюйте.

# speaker: Roman
# emotion: neutral
# textbox: violet
Гарного дня.

# speaker: Mark
# emotion: neutral
# textbox: red
Так, дякую. До побачення.

-> after_roman


=== after_roman ===
# bg: hallway
# clear: all
# textbox: black

Марк вийшов з кабінету й зачинив за собою двері. У коридорі на нього чекала Арія.

# show: Mark,left,fullbody
# show: Ariya,right,fullbody

# speaker: Ariya
# emotion: neutral
# textbox: pink
Ну як воно?

# speaker: Mark
# emotion: neutral
# textbox: red
Добре.

{told_ariya_truth:
    # speaker: Mark
    # emotion: neutral
    # textbox: pink
    Дав список літератури почитати.

    # speaker: Ariya
    # emotion: neutral
    # textbox: pink
    Ну добре.
}

# speaker: Ariya
# emotion: smile
# textbox: pink
День сьогодні такий теплий. Не хочеш пройтись?

-> walkornot


=== walkornot ===
+ [Піти погуляти з Арією]
    -> confirm_walk

+ [Відмовитись, повернутись у кімнату]
    -> confirm_room


=== confirm_walk ===
# speaker: none
# textbox: black
Якщо Марк зараз піде гуляти з Арією, він повернеться до кімнати пізніше. Повернутися до цього вибору не можна.

+ [Так, піти погуляти]
    ~ walked_with_ariya = true
    ~ ariya_points += 2
    -> walk_with_ariya

+ [Ні, ще подумати]
    -> walkornot


=== confirm_room ===
# speaker: none
# textbox: black
Якщо Марк повернеться у кімнату, сьогоднішня прогулянка з Арією буде недоступна. Повернутися до цього вибору не можна.

+ [Так, повернутися в кімнату]
    -> evening_room

+ [Ні, ще подумати]
    -> walkornot


=== walk_with_ariya ===
# bg: walk1
# clear: all
# show: Mark,left,fullbody
# show: Ariya,right,fullbody
# textbox: black

Вони неквапливо вийшли з університетського корпусу. Після шумних коридорів вулиця здалася майже тихою.

# speaker: Ariya
# emotion: smile
# textbox: pink
Знаєш, що найскладніше в тому, щоб бути старостою?

# speaker: Mark
# emotion: other2
# textbox: red
Не відповідати на повідомлення після опівночі?

# speaker: Ariya
# emotion: neutral
# textbox: pink
Найскладніше — виглядати так, ніби ти все контролюєш.

+ [Поспівчувати Арії]
    ~ ariya_points += 1
    -> starosta_support

+ [Пожартувати, що вона створена для цього]
    -> starosta_joke

=== starosta_support ===
# speaker: Mark
# emotion: neutral
# textbox: red
Якщо чесно, ти справляєшся краще, ніж більшість людей на твоєму місці.

# speaker: Ariya
# emotion: smile
# textbox: pink
Пізно. Я вже запам'ятала.

-> walk_scooter

=== starosta_joke ===
# speaker: Mark
# emotion: smile
# textbox: red
Мені здається, ти створена для цього. У тебе навіть погляд є такий: «зараз усіх організую».

# speaker: Ariya
# emotion: other2
# textbox: pink
Це не погляд. Це наслідок групового чату.

-> walk_scooter

=== walk_scooter ===
# bg: walk1
# clear: all
# show: Mark,left,fullbody
# show: Ariya,right,fullbody
# textbox: black

Вони звернули на алею. Попереду хлопець на електросамокаті ледь не врізався в смітник.

# speaker: Ariya
# emotion: smile
# textbox: pink
Іноді мені здається, що деякі люди живуть виключно на удачі.

+ [Засудити]
    ~ ariya_points += 1
    -> scooter_judge

+ [Сказати, що це природний відбір]
    -> scooter_selection

=== scooter_judge ===
# speaker: Mark
# emotion: neutral
# textbox: red
Телефон у руці, самокат під ногами, смітник попереду. Це заявка на травмпункт.

# speaker: Ariya
# emotion: smile
# textbox: pink
Погоджуюсь, самокатистів в пекло.

-> stupid_injury_question

=== scooter_selection ===
# speaker: Mark
# emotion: other2
# textbox: red
Природний відбір сьогодні був зайнятий і просто попередив його смітником.

# speaker: Ariya
# emotion: smile
# textbox: pink
Добре, звучить майже філософськи.

-> stupid_injury_question

=== stupid_injury_question ===
# speaker: Ariya
# emotion: smile
# textbox: pink
Який найдурніший спосіб отримати травму ти знаєш?

+ [Власний досвід]
    ~ ariya_points += 1
    -> injury_mark

+ [Історія про Лева]
    ~ lev_points += 1
    -> injury_lev

+ [Не хочу це згадувати]
    -> injury_hide

=== injury_mark ===
# speaker: Mark
# emotion: other2
# textbox: red
Колись я вдарився об дверцята шафи, які сам же залишив відкритими.

# speaker: Ariya
# emotion: smile
# textbox: pink
Ось тепер звучить.

-> walk_park_begin

=== injury_lev ===
# speaker: Mark
# emotion: smile
# textbox: red
Лев якось став на стілець за книжкою і впав так, ніби це була частина плану.

# speaker: Ariya
# emotion: smile
# textbox: pink
Це дуже схоже на Лева.

-> walk_park_begin

=== injury_hide ===
# speaker: Mark
# emotion: thinking
# textbox: red
Не хочу це згадувати.

# speaker: Ariya
# emotion: smile
# textbox: pink
Цього достатньо. Я вже бачу трагедію в трьох актах.

-> walk_park_begin

=== walk_park_begin ===
# bg: walk2
# clear: all
# show: Mark,left,neutral
# show: Ariya,right,neutral
# textbox: black

Вони дійшли до невеликого парку неподалік університету. Доріжка тягнулася між деревами до фонтана.

# speaker: Ariya
# emotion: smile
# textbox: pink
Давай гру. Треба вигадати історію випадковому перехожому.

+ [Вигадати смішну історію]
    ~ ariya_points += 1
    -> passerby_funny

+ [Вигадати сумну історію]
    -> passerby_sad

+ [Вигадати загадкову історію]
    ~ ariya_points += 1
    -> passerby_mystery

=== passerby_funny ===
# speaker: Mark
# emotion: smile
# textbox: red
Він три години шукає свою аудиторію. Кава вже третя.

# speaker: Ariya
# emotion: smile
# textbox: pink
Це документальний фільм про перший курс.

-> meet_or_skip_lev

=== passerby_sad ===
# speaker: Mark
# emotion: thinking
# textbox: red
Можливо, він просто не хоче йти туди, куди йде.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Іноді сумні версії теж потрібні.

-> meet_or_skip_lev

=== passerby_mystery ===
# speaker: Mark
# emotion: thinking
# textbox: red
Насправді він не студент. Кава — прикриття, а голуби його вже тричі намагалися зупинити.

# speaker: Ariya
# emotion: smile
# textbox: pink
Голуби як секретна служба університету? Добре, ця версія мені подобається.

-> meet_or_skip_lev

=== meet_or_skip_lev ===
# textbox: black

Біля клумби Марк помітив Лева. Він стояв із напоєм у руці й дивився вниз, наче знайшов щось дуже важливе.

# speaker: Ariya
# emotion: thinking
# textbox: pink
Здається, твій сусід знайшов сенс життя біля клумби.

+ [Підійти до Лева]
    ~ met_lev_on_walk = true
    ~ lev_points += 1
    -> meet_lev_walk

+ [Не підходити, пройти далі з Арією]
    ~ ariya_points += 1
    -> ariya_only_route

=== meet_lev_walk ===
# show: Lev,center,fullbody

# speaker: Mark
# emotion: neutral
# textbox: red
Леве?

# speaker: Lev
# emotion: neutral
# textbox: blue
Тихо. Не рухайтесь різко.

# bg: walk_cat
# clear: all
# show: Mark,left,neutral
# show: Ariya,right,neutral
# show: Lev,center,fullbody
# textbox: black

Біля клумби сидів невеликий сірий кіт.

# speaker: Lev
# emotion: thinking
# textbox: blue
Кіт.

+ [Обережно погладити кота]
    ~ lev_points += 2
    -> cat_pet

+ [Пожартувати над Левом]
    -> cat_joke

+ [Сфотографувати кота для Арії]
    ~ ariya_points += 1
    -> cat_photo

=== cat_pet ===
# textbox: black

Марк обережно простягнув руку. Кіт підійшов ближче й торкнувся носом долоні.

# speaker: Lev
# emotion: smile
# textbox: blue
Тепер це хороша прогулянка.

-> lev_after_cat

=== cat_joke ===
# speaker: Mark
# emotion: smile
# textbox: red
Леве, якщо ти колись загубишся, ми просто перевіримо всі місця з котами.

# speaker: Lev
# emotion: neutral
# textbox: blue
Правильна стратегія.

-> lev_after_cat

=== cat_photo ===
# textbox: black

Марк сфотографував кота біля клумби.

# speaker: Ariya
# emotion: smile
# textbox: pink
Надішлеш мені фото?

# speaker: Mark
# emotion: smile
# textbox: red
Так.

-> lev_after_cat

=== lev_after_cat ===
# bg: walk2
# clear: all
# show: Mark,left,neutral
# show: Ariya,right,neutral
# show: Lev,center,fullbody
# textbox: black

Кіт ліг під лавкою у тіні. Вони відійшли від клумби й рушили повільніше.

# speaker: Ariya
# emotion: thinking
# textbox: pink
Ви з Левом завжди такі?

+ [Так, це наша нормальна форма спілкування]
    ~ lev_points += 2
    -> always_like_this_normal

+ [Ні, сьогодні ми ще стримані]
    ~ lev_points += 1
    -> always_like_this_restrained

+ [Це Лев погано на мене впливає]
    ~ ariya_points += 1
    -> always_like_this_lev_fault

=== always_like_this_normal ===
# speaker: Mark
# emotion: neutral
# textbox: red
Так. Це наша нормальна форма спілкування.

# speaker: Lev
# emotion: neutral
# textbox: blue
Будь-яка інша форма була б підозрілою.

-> who_of_us_game

=== always_like_this_restrained ===
# speaker: Mark
# emotion: smile
# textbox: red
Ні, сьогодні ми ще стримані.

# speaker: Lev
# emotion: neutral
# textbox: blue
Повна версія доступна тільки після підписання відмови від претензій.

-> who_of_us_game

=== always_like_this_lev_fault ===
# speaker: Mark
# emotion: other2
# textbox: red
Це Лев погано на мене впливає.

# speaker: Lev
# emotion: neutral
# textbox: blue
Я називаю це розвитком особистості.

-> who_of_us_game

=== who_of_us_game ===
# speaker: Ariya
# emotion: smile
# textbox: pink
Добре. Тоді гра. Хто з нас найшвидше вижив би під час зомбі-апокаліпсису?

+ [Лев]
    ~ lev_points += 1
    -> zombie_lev

+ [Арія]
    ~ ariya_points += 1
    -> zombie_ariya

+ [Марк]
    -> zombie_mark

=== zombie_lev ===
# speaker: Mark
# emotion: neutral
# textbox: red
Лев.

# speaker: Lev
# emotion: neutral
# textbox: blue
Я б просто не вийшов з кімнати.

-> wedding_question

=== zombie_ariya ===
# speaker: Mark
# emotion: neutral
# textbox: red
Арія. Вона б організувала безпечну зону і таблицю чергувань.

# speaker: Ariya
# emotion: smile
# textbox: pink
Я б тебе все одно знайшла, Леве.

-> wedding_question

=== zombie_mark ===
# speaker: Mark
# emotion: other2
# textbox: red
Я. Знайти тихе місце, запаси їжі й не геройствувати.

# speaker: Lev
# emotion: neutral
# textbox: blue
Нудно. Але працює.

-> wedding_question

=== wedding_question ===
# speaker: Ariya
# emotion: smile
# textbox: pink
Хто з нас першим запізниться на власне весілля?

+ [Лев]
    ~ lev_points += 1
    -> wedding_lev

+ [Марк]
    -> wedding_mark

+ [Арія]
    ~ ariya_points += 1
    -> wedding_ariya

=== wedding_lev ===
# speaker: Mark
# emotion: smile
# textbox: red
Лев.

# speaker: Lev
# emotion: neutral
# textbox: blue
Це називається оптимістичне планування.

-> strange_story_question

=== wedding_mark ===
# speaker: Mark
# emotion: other2
# textbox: red
Я. Пішов би раніше, щоб не запізнитись, а потім відволікся б.

# speaker: Ariya
# emotion: smile
# textbox: pink
Неочікувано чесно.

-> strange_story_question

=== wedding_ariya ===
# speaker: Mark
# emotion: smile
# textbox: red
Арія. Бо перед виходом ще перевірила б, чи всі гості знають, куди йти.

# speaker: Ariya
# emotion: smile
# textbox: pink
Це не запізнення. Це відповідальність.

-> strange_story_question

=== strange_story_question ===
# speaker: Ariya
# emotion: neutral
# textbox: pink
Хто з нас найімовірніше влипне в дивну історію?

+ [Лев]
    -> strange_lev

+ [Арія]
    ~ ariya_points += 1
    -> strange_ariya

+ [Марк]
    -> strange_mark

=== strange_lev ===
# speaker: Mark
# emotion: smile
# textbox: red
Лев. Він уже сьогодні майже приєднався до котячого культу.

# speaker: Lev
# emotion: neutral
# textbox: blue
Не культ. Спільнота за інтересами.

-> awkward_question

=== strange_ariya ===
# speaker: Mark
# emotion: neutral
# textbox: red
Арія. Бо ти намагаєшся всіх рятувати.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Це було занадто точно.

-> awkward_question

=== strange_mark ===
# speaker: Mark
# emotion: thinking
# textbox: red
Мабуть, я.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Ти так сказав, ніби вже влип.

-> awkward_question

=== awkward_question ===
# speaker: Ariya
# emotion: thinking
# textbox: pink
Якщо щось справді станеться... кому ти подзвониш першим?

+ [Арії]
    ~ ariya_points += 2
    -> call_ariya_first

+ [Леву]
    ~ lev_points += 2
    -> call_lev_first

+ [Спробую сам розібратися]
    -> call_self_first

=== call_ariya_first ===
# speaker: Mark
# emotion: neutral
# textbox: red
Арії.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Тоді я постараюсь не вимкнути телефон.

-> shared_joke

=== call_lev_first ===
# speaker: Mark
# emotion: neutral
# textbox: red
Леву.

# speaker: Lev
# emotion: smile
# textbox: blue
Приємна помилка. Але все одно не візьму трубку.

-> shared_joke

=== call_self_first ===
# speaker: Mark
# emotion: thinking
# textbox: red
Спробую сам розібратися.

# speaker: Lev
# emotion: thinking
# textbox: blue
Поганий план. Звучить дуже по-марківськи.

-> shared_joke

=== shared_joke ===
# textbox: black

Поруч проходив Маркіян Захарович із папкою під рукою. Арія стишила голос.

# speaker: Ariya
# emotion: smile
# textbox: pink
Він точно живе в університеті.

# speaker: Mark
# emotion: smile
# textbox: red
У підвалі кафедри. Там у нього чайник і шафа з додатковими дедлайнами.

# speaker: Lev
# emotion: neutral
# textbox: blue
Дедлайни він тримає в сейфі.

-> lev_walk_end_choice

=== lev_walk_end_choice ===
# textbox: black

Сонце почало хилитися нижче. Парк поступово ставав тихішим.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Я ще зайду в магазин дорогою додому.

# speaker: Lev
# emotion: neutral
# textbox: blue
Мій соціальний ліміт вичерпався на слові «гра».

# speaker: Ariya
# emotion: smile
# textbox: pink
Добре. Тоді побачимось завтра.

# hide: Ariya
# show: Mark,left,neutral
# show: Lev,right,neutral
# textbox: black

Арія помахала їм рукою й пішла в бік магазину. Марк разом із Левом повернув у напрямку гуртожитку.

-> evening_room

=== ariya_only_route ===
# bg: walk2
# clear: all
# show: Mark,left,neutral
# show: Ariya,right,neutral
# textbox: black

Марк і Арія обійшли фонтан і сіли на лавку під деревом.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Ким ти хотів бути в дитинстві?

+ [Науковцем]
    ~ study_points += 1
    ~ ariya_points += 1
    -> dream_scientist

+ [Мандрівником]
    ~ ariya_points += 1
    -> dream_traveler

+ [Письменником]
    -> dream_writer

+ [Не пам'ятаю]
    -> dream_dont_remember

=== dream_scientist ===
# speaker: Mark
# emotion: neutral
# textbox: red
Науковцем. Але таким, як у фільмах: із дошкою, формулами й драматичним поглядом у вікно.

# speaker: Ariya
# emotion: smile
# textbox: pink
Тобто головне було не відкриття, а атмосфера?

-> ariya_own_dream

=== dream_traveler ===
# speaker: Mark
# emotion: thinking
# textbox: red
Мандрівником. Мені здавалося, що можна просто зібрати рюкзак і піти дивитися світ.

# speaker: Ariya
# emotion: smile
# textbox: pink
Дорослішання звучить трагічно.

-> ariya_own_dream

=== dream_writer ===
# speaker: Mark
# emotion: thinking
# textbox: red
Письменником. Мені подобалося вигадувати історії, але я майже нікому їх не показував.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Дивні історії часто найкращі.

~ ariya_points += 1
-> ariya_own_dream

=== dream_dont_remember ===
# speaker: Mark
# emotion: neutral
# textbox: red
Не пам'ятаю. Мабуть, у мене не було чогось одного.

# speaker: Ariya
# emotion: smile
# textbox: pink
Або було, але ти навчився не сприймати це серйозно.

-> ariya_own_dream

=== ariya_own_dream ===
# speaker: Ariya
# emotion: thinking
# textbox: pink
Я колись хотіла проєктувати будинки. Місця, де людям було б добре.

# speaker: Mark
# emotion: thinking
# textbox: red
Чому передумала?

# speaker: Ariya
# emotion: other2
# textbox: pink
Здалося, що треба обрати щось практичніше.

+ [Сказати, що їй би це пасувало]
    ~ ariya_points += 2
    -> ariya_architecture_support_new

+ [Запитати, чи вона шкодує]
    ~ ariya_points += 1
    -> ariya_architecture_regret_new

+ [Пожартувати про будинок без дедлайнів]
    -> ariya_architecture_joke_new

=== ariya_architecture_support_new ===
# speaker: Mark
# emotion: neutral
# textbox: red
Мені здається, тобі б це пасувало. Ти й так думаєш, як зробити людям зручніше.

# speaker: Ariya
# emotion: smile
# textbox: pink
Це було... дуже мило, Марку.

-> best_day_question

=== ariya_architecture_regret_new ===
# speaker: Mark
# emotion: neutral
# textbox: red
Ти шкодуєш?

# speaker: Ariya
# emotion: neutral
# textbox: pink
Іноді. Але не так, щоб хотілося все кинути.

-> best_day_question

=== ariya_architecture_joke_new ===
# speaker: Mark
# emotion: smile
# textbox: red
Ти могла б спроєктувати будинок без дедлайнів.

# speaker: Ariya
# emotion: smile
# textbox: pink
Це вже не архітектура. Це фантастика.

-> best_day_question

=== best_day_question ===
# speaker: Ariya
# emotion: neutral
# textbox: pink
А який у тебе був найкращий день за останній рік?

+ [Тихий день без проблем]
    ~ ariya_points += 1
    -> best_day_quiet

+ [День із друзями]
    ~ ariya_points += 1
    -> best_day_friends

+ [Не можу згадати]
    -> best_day_cant_remember

=== best_day_quiet ===
# speaker: Mark
# emotion: neutral
# textbox: red
Мабуть, якийсь тихий день. Без термінових справ і без відчуття, що я щось забув.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Іноді спокій здається розкішшю.

-> ariya_personal_moment

=== best_day_friends ===
# speaker: Mark
# emotion: smile
# textbox: red
День, коли ми з Левом весь вечір дивилися дурні відео.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Іноді такі дні важливіші за корисні.

-> ariya_personal_moment

=== best_day_cant_remember ===
# speaker: Mark
# emotion: thinking
# textbox: red
Я не можу згадати.

# speaker: Ariya
# emotion: thinking
# textbox: pink
Може, це означає, що треба зробити новий.

~ ariya_points += 1
-> ariya_personal_moment

=== ariya_personal_moment ===
# speaker: Ariya
# emotion: thinking
# textbox: pink
Іноді мені здається, що всі навколо вже знають, куди рухаються. А я просто намагаюся не відстати.

# speaker: Mark
# emotion: neutral
# textbox: red
Аріє...

+ [Підтримати її серйозно]
    ~ ariya_points += 2
    -> ariya_support_serious

+ [Сказати, що вона не має бути ідеальною]
    ~ ariya_points += 2
    -> ariya_not_perfect

+ [М'яко пожартувати, щоб розрядити атмосферу]
    ~ ariya_points += 1
    -> ariya_soft_joke

=== ariya_support_serious ===
# speaker: Mark
# emotion: neutral
# textbox: red
Не звучить драматично. Звучить так, ніби ти довго тримала це в собі.

# speaker: Ariya
# emotion: smile
# textbox: pink
Домовились. Тоді я спробую просто бути собою.

-> ariya_only_end

=== ariya_not_perfect ===
# speaker: Mark
# emotion: neutral
# textbox: red
Ти не маєш бути ідеальною, щоб люди могли на тебе покладатися.

# speaker: Ariya
# emotion: smile
# textbox: pink
Дякую.

-> ariya_only_end

=== ariya_soft_joke ===
# speaker: Mark
# emotion: other2
# textbox: red
Якщо всі навколо знають, куди рухаються, то вони дуже добре приховують паніку.

# speaker: Ariya
# emotion: smile
# textbox: pink
Це дивно втішає.

-> ariya_only_end

=== ariya_only_end ===
# textbox: black

Вони ще трохи посиділи на лавці. Коли піднялися, Арія виглядала трохи легше.

# speaker: Ariya
# emotion: neutral
# textbox: pink
Дякую, що пройшовся зі мною.

# speaker: Mark
# emotion: smile
# textbox: red
Це було приємно.

# hide: Ariya
# textbox: black

Арія попрощалася біля входу до гуртожитку й пішла у свій бік. Марк видихнув і зайшов усередину.

-> evening_room

=== evening_room ===
# bg: evening
# clear: all
# textbox: black
# sfx: door

Марк повернувся до гуртожитку, закриваючи двері ногою.

# show: Mark,left,neutral

# speaker: Mark
# emotion: neutral
# textbox: red
Ну нарешті.

# textbox: black
На екрані ноутбука досі світилась іконка нового листа.

Книги з бібліотеки лежали на столі, ніби чекали своєї черги.

Марк не відкрив повідомлення одразу. Після всього дня йому потрібно було хоча б кілька хвилин тиші.

~ got_second_mail = true
# save: progress

-> room_caesar_start

=== room_caesar_start ===
# bg: room_evening
# clear: all
# textbox: black

В кімнаті було тихіше, ніж у коридорах університету.

На столі стояв ноутбук. Іконка пошти досі світилася так, ніби хтось спеціально не давав Марку забути про лист.

{walked_with_ariya:
    {met_lev_on_walk:
        # show: Mark,left,neutral
        # show: Lev,right,neutral
        Марк і Лев повернулися майже разом. Арія попрощалася біля входу, а вони мовчки піднялися до кімнати.

        Лев кинув рюкзак біля ліжка, а Марк поклав книги на стіл поруч із ноутбуком.

        # speaker: Lev
        # emotion: neutral
        # textbox: blue
        Ти після прогулянки виглядаєш так, ніби тебе не відпустило.

        # speaker: Mark
        # emotion: other2
        # textbox: red
        Це просто мій звичайний вигляд.
    - else:
        # show: Mark,left,neutral
        Марк повернувся сам. Він поставив книги на стіл і тільки встиг сісти, як у дверях провернувся ключ.

        # sfx: door
        # show: Lev,right,neutral
        Лев зайшов у кімнату, поставив стакан з напоєм на підвіконня і скинув рюкзак біля ліжка.

        # speaker: Lev
        # emotion: neutral
        # textbox: blue
        Ти вже тут.

        # speaker: Mark
        # emotion: neutral
        # textbox: red
        Ага.
    }
- else:
    # show: Mark,left,neutral
    # show: Lev,right,neutral
    Лев уже був у кімнаті. Він сидів на своєму ліжку, гортав телефон і лише коротко глянув на Марка, коли той зайшов.

    Марк розклав речі біля ноутбука, а Лев мовчки посунув свій рюкзак з проходу.

    # speaker: Lev
    # emotion: neutral
    # textbox: blue
    Ти швидко.

    # speaker: Mark
    # emotion: neutral
    # textbox: red
    Не хотілось десь зависати.
}

# textbox: black

Вони розклали речі. Лев удавав, що повністю зайнятий телефоном, але час від часу кидав погляд на Марка.

На екрані ноутбука світилася іконка нового повідомлення.

# speaker: Mark
# emotion: thinking
# textbox: red
Треба подивитися, що там.

{told_lev_about_message:
    -> second_mail_choice_known
- else:
    -> second_mail_choice_hidden_first
}

=== second_mail_choice_known ===
+ [Розказати Леву про другий лист]
    ~ told_lev_about_second_mail = true
    ~ lev_points += 1
    -> tell_lev_second_mail

+ [Не розказувати про другий лист]
    ~ told_lev_about_second_mail = false
    ~ fear += 1
    -> hide_second_mail

=== second_mail_choice_hidden_first ===
+ [Розказати Леву про всі листи]
    ~ told_lev_about_message = true
    ~ told_lev_about_second_mail = true
    ~ lev_points += 2
    -> tell_lev_all_mails

+ [Знову нічого не казати]
    ~ told_lev_about_second_mail = false
    ~ fear += 1
    -> hide_second_mail

=== tell_lev_second_mail ===
# speaker: Mark
# emotion: thinking
# textbox: red
Мені знову прийшов лист.

# speaker: Lev
# emotion: neutral
# textbox: blue
Звісно. Бо одного разу було мало.

# speaker: Mark
# emotion: other2
# textbox: red
Я ще не відкривав. Але іконка висить з пари.

# speaker: Lev
# emotion: neutral
# textbox: blue
Тоді відкривай. Але якщо там знову щось дивне — не роби вигляд, що все нормально.

-> open_second_mail

=== tell_lev_all_mails ===
# speaker: Mark
# emotion: thinking
# textbox: red
Леве... я не все тобі сказав.

# speaker: Lev
# emotion: neutral
# textbox: blue
Це вже звучить погано.

# speaker: Mark
# emotion: other2
# textbox: red
Перший лист був не просто спамом. Потім був таймер. Потім фото. А сьогодні прийшло ще одне повідомлення.

# speaker: Lev
# emotion: angry
# textbox: blue
І ти вирішив сказати це зараз?

# speaker: Mark
# emotion: thinking
# textbox: red
Я не знав, що з цим робити.

# speaker: Lev
# emotion: neutral
# textbox: blue
Почни з того, що відкрий лист. А потім не бреши хоча б п'ять хвилин.

-> open_second_mail

=== hide_second_mail ===
# textbox: black

Марк нічого не сказав. Він лише потягнув ноутбук ближче й відкрив кришку.

Лев помітив рух, але не став питати.

-> open_second_mail

=== open_second_mail ===
# laptop: open
# mail_icon: new
# add_mail: 2
# next_after_mail: after_second_mail_read
# protected_file_next: protected_file_opened

-> DONE

=== after_second_mail_read ===
# laptop: close
# bg: room_evening
# clear: all
# show: Mark,left,thinking
# show: Lev,right,neutral
# textbox: black

Марк перечитав повідомлення ще раз.

У листі був набір букв і запаролений файл.


{told_lev_about_second_mail:
    # speaker: Lev
    # emotion: neutral
    # textbox: blue
    Погладь кота? Рекомендація звісно чудова, але що це в біса має оззначати.

    # speaker: Mark
    # emotion: thinking
    # textbox: red
    Роман казав почати з класичних шифрів.
- else:
    # textbox: black
    Марк прикрив текст рукою, роблячи вигляд, що просто читає старий файл.
}

# textbox: black


На столі лежали конспекти, біля ліжка валялися речі, а на книжковій полиці стояли старі підручники, які Марк давно не відкривав.

# room_next: after_room_search
# room_mode: open
# room_bookshelf_available: true
# protected_file_next: protected_file_opened

-> DONE

=== after_room_search ===
# bg: room_evening
# clear: all
# show: Mark,left,thinking
# show: Lev,right,neutral
# textbox: black

Після огляду кімнати Марк повернувся до ноутбука.


# speaker: Mark
# emotion: thinking
# textbox: red
Добре. Спробую Шифр Цезаря. 

# speaker: Mark
# emotion: other2
# textbox: red
Тепер треба відкрити файл.

# laptop: open
# protected_file_next: protected_file_opened

-> DONE

=== protected_file_opened ===
# laptop: close
# bg: room_evening
# clear: all
# show: Mark,left,thinking
# show: Lev,right,neutral
# textbox: black

Файл нарешті відкрився.



~ opened_locked_file = true
~ solved_cipher_2 = true
~ file_opened = true
# save: progress

-> end_of_day

=== end_of_day ===
# bg: room_evening
# clear: all
# show: Mark,left,neutral
# show: Lev,right,neutral
# textbox: black

Вечір остаточно опустився на кімнату.

Ноутбук стояв відкритий, але Марк уже не поспішав торкатися клавіатури.

Пора спати.

# save: progress

-> DONE

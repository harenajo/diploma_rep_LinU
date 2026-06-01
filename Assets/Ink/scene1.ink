VAR prologue_timer = false
VAR class_route = false
VAR lev_points = 0
VAR study_points = 0
VAR fear = 0
VAR told_lev_about_message = false
VAR file_opened = false
VAR opened_locked_file = false
VAR solved_cipher_2 = false
VAR caesar_mistakes = 0
VAR failed_cipher_attempts = 0

=== start ===
# bg: night
# clear: all
# textbox: black

Монотонний шум вентилятора заповнював кімнату гуртожитку.

За вікном давно стемніло, а Марк уже майже засинав прямо за ноутбуком.

# show: Lev,right,fullbody
# speaker: Lev
# emotion: fullbody
# textbox: blue
Лягай спати вже.

# show: Mark,left,fullbody
# speaker: Mark
# emotion: fullbody
# textbox: red
Треба готуватись до тесту. Ну ще півгодини і ляжу.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Ти це казав три години тому.

# speaker: Mark
# emotion: fullbody
# textbox: red
Добре. Ще один абзац і справді лягаю.

-> night_message

=== night_message ===
# textbox: black
# sfx: notification

На екрані з’явилось нове повідомлення.

Невідомий відправник.

# speaker: Mark
# emotion: fullbody
# textbox: red
Спам якийсь...

# speaker: Lev
# emotion: fullbody
# textbox: blue
Не відкривай.

# textbox: black

Марк відкрив лист. Екран мигнув.

Вікно само закрилось.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Я ж сказав не відкривай.

# textbox: black

Марк закрив ноутбук і пішов спати. Досить з нього сьогодні. Допрацювався.

-> morning_scene

=== morning_scene ===
# bg: day
# clear: all
# textbox: black

Наступного дня Марк прокинувся ближче до обіду.

Лев уже стояв біля дверей.

# show: Lev,right,fullbody
# speaker: Lev
# emotion: fullbody
# textbox: blue
На пари не йдеш?

+ [Піти на пари]
    ~ class_route = true
    ~ study_points += 1
    -> go_to_classes

+ [Залишитися в кімнаті]
    ~ class_route = false
    -> stay_in_room

=== stay_in_room ===
# bg: day
# clear: all
# show: Mark,left,neutral
# show: Lev,right,fullbody
# textbox: black

# speaker: Mark
# emotion: neutral
# textbox: red
Не сьогодні.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Як хочеш.

+ [Попросити Лева написати, якщо буде щось важливе]
    ~ lev_points += 1
    -> ask_lev_updates

+ [Нічого не казати]
    -> lev_leaves_room

=== ask_lev_updates ===
# speaker: Mark
# emotion: neutral
# textbox: red
Якщо там буде щось важливе, напишеш?

# speaker: Lev
# emotion: fullbody
# textbox: blue
Ти серйозно просиш мене бути твоєю системою сповіщень?

# speaker: Mark
# emotion: other2
# textbox: red
Ти вже стоїш біля дверей. Це майже офіційна посада.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Ладно. Але без гарантій і без техпідтримки.

-> lev_leaves_room

=== lev_leaves_room ===
# bg: day
# textbox: black
# sfx: door

Лев вийшов із кімнати.

Марк залишився сам і знову сів за ноутбук.

# laptop: open
# mail_icon: new
# add_mail: 1
# next_after_mail: after_read_mail_room

Ноутбук засвітився новим повідомленням.

-> DONE

=== after_read_mail_room ===
# laptop: close
# bg: day
# clear: all
# show: Mark,left,other2
# textbox: yellow

~ prologue_timer = true

# speaker: Mark
# emotion: other2
# textbox: red
Що за..?

# speaker: Mark
# emotion: thinking
# textbox: red
...

# speaker: Mark
# emotion: other
# textbox: red
Безглуздя якесь.

# textbox: black

Марк закрив ноутбук.

Ще йому цих приколів не вистачало.

-> evening_room_lev_returns

=== evening_room_lev_returns ===
# bg: evening
# clear: all
# textbox: black
# sfx: door

Лев повернувся з занять.

Він мовчки розклав речі біля ліжка, кинув рюкзак на стілець і тільки тоді подивився на Марка.

# show: Mark,left,neutral
# show: Lev,right,other2

# speaker: Mark
# emotion: neutral
# textbox: red
Хей.

# speaker: Lev
# emotion: other2
# textbox: blue
Ага.

# speaker: Mark
# emotion: neutral
# textbox: red
Що цікавого було?

# speaker: Lev
# emotion: angry
# textbox: blue
Може, сам хоч раз сходив би — побачив би щось.

# speaker: Lev
# emotion: angry
# textbox: blue
Роман вже питається за тебе. Ти хоч пам’ятаєш, що ти в нього курсову пишеш?

# speaker: Mark
# emotion: other2
# textbox: red
Ха-ха. Та-та. Я пам’ятаю.

-> common_tell_choice

=== go_to_classes ===
# bg: day
# clear: all
# textbox: black
# sfx: door

Марк швидко зібрав речі й вийшов разом із Левом.

# show: Mark,left,fullbody
# show: Lev,right,fullbody

# speaker: Lev
# emotion: fullbody
# textbox: blue
Дивно. Я вже думав, ти сьогодні точно залишишся.

# speaker: Mark
# emotion: fullbody
# textbox: red
...

-> classes_continue

=== classes_continue ===
# speaker: Lev
# emotion: fullbody
# textbox: blue
Відмітимо цей день у календарі.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Марк нарешті пішов на пари.

# speaker: Lev
# emotion: fullbody
# textbox: blue
Пан Роман дуже радий буде.

# bg: classroom
# clear: all
# textbox: black

На першій парі був той самий страшний тест, до якого Марк готувався усю ніч.

На наступній парі він уже майже не слухав викладача, граючи в дурня онлайн на своєму ноутбуці.

# sfx: notification

На екрані було нове повідомлення від невідомого відправника.

# laptop: open
# mail_icon: new
# add_mail: 1
# next_after_mail: after_read_mail_classroom

-> DONE

=== after_read_mail_classroom ===
# laptop: close
# bg: classroom
# clear: all
# show: Mark,left,other2
# show: Lev,right,neutral
# textbox: red

~ prologue_timer = true

# speaker: Mark
# emotion: other2
# textbox: red
Що за..?

# speaker: Mark
# emotion: thinking
# textbox: red
...

# speaker: Mark
# emotion: other
# textbox: red
Безглуздя якесь.

# textbox: black

До кінця пари залишалося ще кілька хвилин. Викладач щось пояснював біля дошки, але Марк уже дивився тільки на екран ноутбука.

Лев сидів поруч і помітив, як Марк різко зблід.

# speaker: Lev
# emotion: neutral
# textbox: blue
Ти нормальний?

# speaker: Mark
# emotion: other2
# textbox: red
Так. Просто... голова заболіла.

# textbox: black

Пара закінчилася швидше, ніж Марк встиг придумати хоч якесь пояснення.

-> class_return_together_room

=== class_return_together_room ===
# bg: evening
# clear: all
# textbox: black
# sfx: door

Марк і Лев повернулися до кімнати разом.

Вони розклали речі майже одночасно: Лев кинув рюкзак біля столу, Марк залишив конспекти поруч із ноутбуком.

# show: Mark,left,thinking
# show: Lev,right,neutral

# speaker: Lev
# emotion: neutral
# textbox: blue
Ти всю дорогу мовчав.

# speaker: Mark
# emotion: thinking
# textbox: red
Я просто втомився.

# speaker: Lev
# emotion: neutral
# textbox: blue
Після того, як побачив щось у ноутбуці?

# textbox: black

Марк відвів погляд.

-> common_tell_choice

=== common_tell_choice ===
+ [Розказати про повідомлення]
    ~ told_lev_about_message = true
    ~ lev_points += 2
    -> common_message_told

+ [Не розказати про повідомлення]
    ~ told_lev_about_message = false
    ~ fear += 1
    -> common_message_hidden

=== common_message_told ===
# textbox: black

Марк кілька секунд мовчав, а потім повернув ноутбук до Лева.

# speaker: Mark
# emotion: other
# textbox: red
Чуєш, мені прийшло дивне повідомлення.

# speaker: Mark
# emotion: thinking
# textbox: red
Там був таймер. І це не схоже на звичайний спам.

# speaker: Lev
# emotion: neutral
# textbox: blue
Покажи.

# textbox: black

Марк показав повідомлення Леву. Таймер показував 3 хвилини 12 секунд.

# speaker: Lev
# emotion: neutral
# textbox: blue
Ну вітаю, що сказати. Безпека в інтернеті — це не твоє.

# speaker: Lev
# emotion: neutral
# textbox: blue
Не відповідай. І більше нічого не відкривай.

-> common_phone_consequence

=== common_message_hidden ===
# textbox: black

Марк не розповів про повідомлення Леву.

Він зробив вигляд, що просто перевіряє старі файли, але пальці все одно тремтіли над клавіатурою.

Лев помітив це, але нічого не сказав.

-> common_phone_consequence

=== common_phone_consequence ===
# bg: evening
# clear: all
# show: Mark,left,other2
# show: Lev,right,angry
# textbox: black

Таймер минув.

# sfx: notification

На телефон Лева прийшло нове повідомлення.

Він відкрив повідомлення. Фотографія. Селфі.

Марк у його кофті — тій, яку Лев не міг знайти місяць.

А в руках енергетик. Його енергетик.

Марк підвівся з місця.

# speaker: Lev
# emotion: angry
# textbox: blue
Це що таке?!

# speaker: Mark
# emotion: other2
# textbox: red
Я-я можу все пояснити.

# speaker: Mark
# emotion: thinking
# textbox: red
Зачекай. А звідки взагалі...

# textbox: black

До Марка прийшло усвідомлення.

Він повільно повернувся до ноутбука.

О-ох.

Проблема.

Він повернувся назад до Лева.

# speaker: Mark
# emotion: thinking
# textbox: red
Слухай, а хто в нас займається шифрами чи щось таке?

# speaker: Lev
# emotion: angry
# textbox: blue
Ну Роман наче.

# speaker: Lev
# emotion: neutral
# textbox: blue
Хах, не вийде ігнорувати його вічність. Завтра він має бути на кафедрі.

# textbox: black

Лев посміхунувся глузливо. Не вийде ігронувати університет вічно, Марчику.

# bg: evening


# save: progress

-> DONE

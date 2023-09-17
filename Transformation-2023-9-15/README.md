# Transformationen der XML-Dateien, 15.9.2023

- `<letterDesc ref="<INDEX-NUMMER">` jetzt `<letterDesc letter="<HKB-NUMMER">`
- `<letterTradition ref="<INDEX-NUMMER>">` jetzt `<letterTradition letter="<HKB-NUMMER>">`
- `<letterText ref="<INDEX-NUMMER>">` jetzt `<letterText letter="<HKB-NUMMER>">`
- `<marginal letter="<INDEX-NUMMER>">` jetzt `<marginal letter="<HKB-NUMMER>">`
- `<intlink letter="<INDEX-NUMMER>">` jetzt `<intlink letter="<HKB-NUMMER>">`
- `<autopsic value="<HKB-NUMMER>" />`-Tag gelöscht
- `<marginal index="<INDEX-NUMMER>">` jetzt entweder
    - `<marginal sort="<SORTIERUNG>">` bei mehreren Kommentaren in derselben Zeile
    - oder gelöscht

XML Character Entities sind dabei zu ihren Unicode-Zeichen expandiert worden, wenn sie nicht Weißraum bedeuten.

# Shows help
default:
    @just --list --justfile '{{ justfile() }}'

# Deploy to current steam RimWorld
deploy:
    #!/usr/bin/env sh
    cp -r ./Mods/* ~/.steam/steam/steamapps/common/RimWorld/Mods/


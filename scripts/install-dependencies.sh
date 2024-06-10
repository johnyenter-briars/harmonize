# Install Python dependencies

uv venv
source .venv/bin/activate
uv pip install -r api/requirements.txt


# Install nodejs dependencies

bun install -g @angular/cli

pushd musictool-frontend > /dev/null
bun install
popd > /dev/null
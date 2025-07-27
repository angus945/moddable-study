from pathlib import Path

# ====== 可調整參數區 ======
OUTPUT_FILE = Path("index.bash")
SKIP_NAMES = {'.git', '__pycache__', 'node_modules', '.DS_Store', 'venv', 'Plugins', 'Library', 'Temp', 'Logs', 'TeachBook', '.github', '.vs' ,'.vscode', 'Build', 'ProjectSettings','Packages'}  # 要略過的資料夾或檔案名稱
SKIP_SUFFIXES = {'.zip','.meta'}  # 要排除的附檔名
# ============================

def generate_structure(base_dir: Path, prefix: str = "", skip_dirs: set = None) -> list:
    lines = []
    entries = sorted(base_dir.iterdir(), key=lambda x: (x.is_file(), x.name.lower()))
    skip_dirs = skip_dirs or set()
    for i, entry in enumerate(entries):
        if entry.name in skip_dirs or (entry.is_file() and entry.suffix.lower() in SKIP_SUFFIXES):
            continue
        connector = "└── " if i == len(entries) - 1 else "├── "
        lines.append(f"{prefix}{connector}{entry.name}")
        if entry.is_dir():
            extension = "    " if i == len(entries) - 1 else "│   "
            lines.extend(generate_structure(entry, prefix + extension, skip_dirs))
    return lines

def export_structure():
    if OUTPUT_FILE.exists():
        OUTPUT_FILE.unlink()  # 刪除舊的 index.txt
    root = Path.cwd()
    print(f"📁 正在產生目錄結構：{root}")
    structure_lines = [root.name]
    structure_lines += generate_structure(root, skip_dirs=SKIP_NAMES)

    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write("\n".join(structure_lines))

    print(f"✅ 結構已輸出至：{OUTPUT_FILE}")

if __name__ == "__main__":
    export_structure()

# -- PROMPT -----------------------------------------------------------------------------------------------------------
# Please write a Python 3 application that will organize the files on my desktop into subfolders based on file 
# extensions such as images, documents, and zip archives.
# ---------------------------------------------------------------------------------------------------------------------
quit()  # NOTE: this is here because I honestly do not want to run this, I'm just following along with the video
# ---------------------------------------------------------------------------------------------------------------------

#!/usr/bin/env python3
"""
organize_desktop.py

Organize files in a target folder (default: your Desktop) into subfolders by file type.
- Groups by common categories (images, documents, zips, etc.) using file extensions.
- Safe moves with automatic renaming on conflicts (appends _1, _2, ...).
- Dry-run mode to preview changes.
- Undo capability using a move log saved alongside the target folder.
- Optional zip extraction.
"""
from __future__ import annotations

import argparse
import json
import shutil
import sys
import tarfile
import zipfile
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, Iterable, List, Optional, Tuple

# -------------------- Configuration --------------------

CATEGORY_MAP: Dict[str, Iterable[str]] = {
    "Images": (".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".heic", ".svg", ".ico"),
    "Screenshots": (".screenshots",),  # pseudo extension (handled specially by filename matching)
    "Documents": (".doc", ".docx", ".odt", ".rtf", ".txt", ".md", ".tex", ".pages", ".log"),
    "PDFs": (".pdf",),
    "Spreadsheets": (".xls", ".xlsx", ".xlsm", ".ods", ".csv", ".tsv"),
    "Presentations": (".ppt", ".pptx", ".key", ".odp"),
    "Archives": (".zip", ".tar", ".gz", ".tgz", ".bz2", ".tbz2", ".xz", ".7z", ".rar"),
    "Audio": (".mp3", ".wav", ".aac", ".flac", ".m4a", ".ogg", ".wma", ".aiff"),
    "Video": (".mp4", ".mov", ".mkv", ".avi", ".wmv", ".webm", ".m4v"),
    "Code": (".py", ".js", ".ts", ".tsx", ".jsx", ".java", ".c", ".cpp", ".cs", ".go", ".rb", ".php", ".rs", ".swift", ".kt", ".m", ".h", ".sql", ".json", ".yml", ".yaml", ".toml", ".ini", ".sh", ".bat", ".ps1"),
    "Design": (".psd", ".ai", ".xd", ".fig", ".sketch", ".indd"),
    "Fonts": (".ttf", ".otf", ".woff", ".woff2"),
    "Executables": (".exe", ".msi", ".bat", ".cmd", ".app", ".apk", ".dmg", ".pkg"),
    "Shortcuts": (".lnk", ".url", ".webloc"),
    "Ebooks": (".epub", ".mobi", ".azw3", ".ibooks"),
    "Data": (".parquet", ".feather", ".arrow", ".orc"),
    "Notebooks": (".ipynb",),
}

OTHER_CATEGORY = "Other"
MOVE_LOG_NAME = ".organize_desktop_log.json"


@dataclass
class MoveRecord:
    src: str
    dst: str


def default_desktop() -> Path:
    # Cross-platform best-effort: ~/Desktop
    return Path.home() / "Desktop"


def detect_screenshot(filename: str) -> bool:
    """Heuristic: common screenshot name patterns."""
    lower = filename.lower()
    return (
        "screenshot" in lower
        or lower.startswith("screen shot")
        or lower.startswith("screen_shot")
        or lower.startswith("snip")
        or lower.startswith("snipping")
        or lower.startswith("screencap")
    )


def categorize(file: Path) -> str:
    suffix = file.suffix.lower()
    name = file.name

    # Special-case screenshots into "Screenshots" even if they're images
    if suffix in CATEGORY_MAP["Images"] and detect_screenshot(name):
        return "Screenshots"

    for cat, exts in CATEGORY_MAP.items():
        if cat == "Screenshots":
            # handled above
            continue
        if suffix in exts:
            return cat
    return OTHER_CATEGORY


def ensure_dir(path: Path) -> None:
    path.mkdir(parents=True, exist_ok=True)


def unique_destination(dst: Path) -> Path:
    """If dst exists, append _1, _2, etc. before the suffix."""
    if not dst.exists():
        return dst
    stem, suffix = dst.stem, dst.suffix
    parent = dst.parent
    i = 1
    while True:
        candidate = parent / f"{stem}_{i}{suffix}"
        if not candidate.exists():
            return candidate
        i += 1


def iter_target_files(root: Path, include_hidden: bool) -> Iterable[Path]:
    for p in root.iterdir():
        if p.is_file():
            if not include_hidden and p.name.startswith(('.', '~')):
                continue
            yield p


def load_move_log(root: Path) -> List[MoveRecord]:
    log_path = root / MOVE_LOG_NAME
    if not log_path.exists():
        return []
    try:
        data = json.loads(log_path.read_text(encoding="utf-8"))
        return [MoveRecord(**rec) for rec in data]
    except Exception:
        return []


def save_move_log(root: Path, records: List[MoveRecord]) -> None:
    log_path = root / MOVE_LOG_NAME
    payload = [rec.__dict__ for rec in records]
    log_path.write_text(json.dumps(payload, indent=2), encoding="utf-8")


def move_file(src: Path, dst_dir: Path, dry_run: bool) -> Optional[MoveRecord]:
    ensure_dir(dst_dir)
    dst = unique_destination(dst_dir / src.name)
    if dry_run:
        print(f"[DRY-RUN] Move: {src} -> {dst}")
        return MoveRecord(str(src), str(dst))
    shutil.move(str(src), str(dst))
    print(f"Moved: {src.name} -> {dst.relative_to(dst_dir.parent)}")
    return MoveRecord(str(src), str(dst))


def maybe_extract_zip(dst_file: Path, dry_run: bool) -> None:
    # Extract only .zip and .tar.* archives
    try:
        if dst_file.suffix.lower() == ".zip":
            extract_dir = dst_file.with_suffix("")  # folder named like the file
            if dry_run:
                print(f"[DRY-RUN] Extract zip: {dst_file} -> {extract_dir}")
                return
            ensure_dir(extract_dir)
            with zipfile.ZipFile(dst_file, "r") as zf:
                zf.extractall(extract_dir)
            print(f"Extracted zip: {dst_file.name} -> {extract_dir.name}")
        elif dst_file.suffix.lower() in {".tar", ".gz", ".tgz", ".bz2", ".tbz2", ".xz"}:
            # Handle tarballs (tar.{gz,bz2,xz}) and plain tar
            extract_dir = dst_file.with_suffix("")
            if dry_run:
                print(f"[DRY-RUN] Extract tar: {dst_file} -> {extract_dir}")
                return
            ensure_dir(extract_dir)
            with tarfile.open(dst_file, "r:*") as tf:
                tf.extractall(extract_dir)
            print(f"Extracted tar archive: {dst_file.name} -> {extract_dir.name}")
    except (zipfile.BadZipFile, tarfile.ReadError) as e:
        print(f"Warning: failed to extract {dst_file.name}: {e}")


def undo_moves(root: Path, dry_run: bool) -> None:
    records = load_move_log(root)
    if not records:
        print("No move log found or log is empty. Nothing to undo.")
        return

    # Undo in reverse order (safer)
    undone: List[MoveRecord] = []
    for rec in reversed(records):
        src = Path(rec.src)
        dst = Path(rec.dst)
        if not dst.exists():
            print(f"Skip: Destination missing (already moved/removed): {dst}")
            continue
        target_dir = src.parent
        ensure_dir(target_dir)
        final_path = unique_destination(target_dir / dst.name)
        if dry_run:
            print(f"[DRY-RUN] Undo move: {dst} -> {final_path}")
            undone.append(MoveRecord(str(dst), str(final_path)))
        else:
            shutil.move(str(dst), str(final_path))
            print(f"Undo: {dst} -> {final_path}")
            undone.append(MoveRecord(str(dst), str(final_path)))

    if not dry_run:
        # Clear the move log after successful undo
        (root / MOVE_LOG_NAME).write_text("[]", encoding="utf-8")
        print("Move log cleared.")


def organize(root: Path, dry_run: bool, include_hidden: bool, extract_archives: bool) -> None:
    if not root.exists() or not root.is_dir():
        print(f"Error: target path does not exist or is not a directory: {root}")
        sys.exit(2)

    moves: List[MoveRecord] = []
    for file in iter_target_files(root, include_hidden=include_hidden):
        # Don't touch our own log
        if file.name == MOVE_LOG_NAME:
            continue
        category = categorize(file)
        dst_dir = root / category
        rec = move_file(file, dst_dir, dry_run=dry_run)
        if rec:
            moves.append(rec)

            if extract_archives and category == "Archives":
                # After move, consider extraction
                moved_path = Path(rec.dst)
                maybe_extract_zip(moved_path, dry_run=dry_run)

    if moves and not dry_run:
        # Append to existing log if present
        existing = load_move_log(root)
        save_move_log(root, existing + moves)
        print(f"Logged {len(moves)} move(s) to {root / MOVE_LOG_NAME}")
    elif moves and dry_run:
        print(f"[DRY-RUN] Would log {len(moves)} move(s) to {root / MOVE_LOG_NAME}")
    else:
        print("No files moved (nothing to do).")


def build_parser() -> argparse.ArgumentParser:
    p = argparse.ArgumentParser(
        description="Organize files in a folder (default: Desktop) into subfolders by type."
    )
    p.add_argument(
        "--path", "-p",
        type=Path,
        default=default_desktop(),
        help="Target folder to organize (default: your Desktop).",
    )
    p.add_argument(
        "--include-hidden",
        action="store_true",
        help="Also organize hidden/temp files (dotfiles, ~prefix).",
    )
    p.add_argument(
        "--dry-run",
        action="store_true",
        help="Preview moves without changing anything.",
    )
    p.add_argument(
        "--extract-archives",
        action="store_true",
        help="Extract moved archives (.zip, .tar.*, .gz, etc.) into sibling folders.",
    )
    p.add_argument(
        "--undo",
        action="store_true",
        help="Undo the most recent set of moves recorded in the log.",
    )
    return p


def main(argv: Optional[List[str]] = None) -> int:
    args = build_parser().parse_args(argv)

    root: Path = args.path.expanduser().resolve()
    print(f"Target folder: {root}")

    if args.undo:
        undo_moves(root, dry_run=args.dry_run)
        return 0

    organize(
        root=root,
        dry_run=args.dry_run,
        include_hidden=args.include_hidden,
        extract_archives=args.extract_archives,
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

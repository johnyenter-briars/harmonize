from pathlib import Path

from PIL import Image


def rescale_jpeg(input_path: Path, output_path: Path, size=(500, 500)):
    img = Image.open(input_path)

    img_resized = img.resize(size, Image.Resampling.LANCZOS)

    img_resized.save(output_path, 'JPEG')


def convert_webp_to_jpeg(input_path: Path, output_path: Path):
    img = Image.open(input_path)

    img.convert('RGB').save(output_path, 'JPEG')

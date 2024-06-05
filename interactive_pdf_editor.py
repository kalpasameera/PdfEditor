import fitz  # PyMuPDF
import tkinter as tk
from tkinter import filedialog, simpledialog, messagebox
from tkinter import Canvas, Scrollbar, HORIZONTAL, VERTICAL
from PIL import Image, ImageTk

class PDFEditor:
    def __init__(self, root):
        self.root = root
        self.root.title("PDF Editor")
        self.pdf_path = None
        self.doc = None
        self.canvas = None
        self.current_page = 0

        # Create GUI components
        self.open_button = tk.Button(root, text="Open PDF", command=self.open_pdf)
        self.open_button.pack(pady=10)

        self.canvas_frame = tk.Frame(root)
        self.canvas_frame.pack(fill=tk.BOTH, expand=1)

        self.canvas = Canvas(self.canvas_frame, bg='white')
        self.canvas.pack(side=tk.LEFT, fill=tk.BOTH, expand=1)

        self.scroll_x = Scrollbar(self.canvas_frame, orient=HORIZONTAL, command=self.canvas.xview)
        self.scroll_x.pack(side=tk.BOTTOM, fill=tk.X)
        self.scroll_y = Scrollbar(self.canvas_frame, orient=VERTICAL, command=self.canvas.yview)
        self.scroll_y.pack(side=tk.RIGHT, fill=tk.Y)

        self.canvas.configure(xscrollcommand=self.scroll_x.set, yscrollcommand=self.scroll_y.set)

        self.add_text_button = tk.Button(root, text="Add Text Box", command=self.add_text_box)
        self.add_text_button.pack(pady=10)
        self.add_text_button.config(state=tk.DISABLED)

        self.save_button = tk.Button(root, text="Save PDF", command=self.save_pdf)
        self.save_button.pack(pady=10)
        self.save_button.config(state=tk.DISABLED)

        self.canvas.bind("<Configure>", self.on_canvas_resize)

    def open_pdf(self):
        self.pdf_path = filedialog.askopenfilename(filetypes=[("PDF files", "*.pdf")])
        if self.pdf_path:
            self.doc = fitz.open(self.pdf_path)
            self.render_page(0)
            messagebox.showinfo("PDF Editor", "PDF opened successfully!")
            self.add_text_button.config(state=tk.NORMAL)
            self.save_button.config(state=tk.NORMAL)

    def render_page(self, page_number):
        self.current_page = page_number
        page = self.doc.load_page(page_number)
        zoom = min(self.canvas.winfo_width() / page.rect.width, self.canvas.winfo_height() / page.rect.height)
        mat = fitz.Matrix(zoom, zoom)
        pix = page.get_pixmap(matrix=mat)
        img = Image.frombytes("RGB", [pix.width, pix.height], pix.samples)
        img = ImageTk.PhotoImage(img)
        self.canvas.image = img
        self.canvas.create_image(0, 0, anchor=tk.NW, image=img)
        self.canvas.config(scrollregion=self.canvas.bbox(tk.ALL))

    def add_text_box(self):
        if self.doc:
            text = simpledialog.askstring("Input", "Enter text to add:")
            if text:
                # Create a draggable text box with a tag
                text_id = self.canvas.create_text(100, 100, text=text, font=('Helvetica', '12'), fill='black', tags="text")
                self.canvas.tag_bind(text_id, "<Button1-Motion>", self.drag_text)
                self.canvas.tag_bind(text_id, "<ButtonRelease-1>", self.drop_text)
                messagebox.showinfo("PDF Editor", "Text box added. You can drag it to position.")

    def drag_text(self, event):
        x, y = event.x, event.y
        self.canvas.coords(self.canvas.find_withtag(tk.CURRENT), x, y)

    def drop_text(self, event):
        pass

    def save_pdf(self):
        if self.doc:
            for text_id in self.canvas.find_withtag("text"):  # Retrieve all items with the tag "text"
                x, y = self.canvas.coords(text_id)
                text = self.canvas.itemcget(text_id, "text")
                page = self.doc.load_page(self.current_page)
                page.insert_text((x, y), text, fontsize=12)

            save_path = filedialog.asksaveasfilename(defaultextension=".pdf", filetypes=[("PDF files", "*.pdf")])
            if save_path:
                self.doc.save(save_path)
                self.doc.close()
                messagebox.showinfo("PDF Editor", "PDF saved successfully!")

    def on_canvas_resize(self, event):
        if self.pdf_path:
            self.render_page(self.current_page)

if __name__ == "__main__":
    root = tk.Tk()
    app = PDFEditor(root)
    root.mainloop()

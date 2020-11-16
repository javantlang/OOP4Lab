﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP4Lab
{
    public partial class PaintBox : Form
    {
        //Создаём список
        LinkedList Mylist;
        //Создаём объект класса graphics для рисования
        private Graphics g;
        //Буфер для bitmap изображения
        private Bitmap bitmapDraw;

        private Model model;
        public PaintBox()
        {
            InitializeComponent();
            //Инициализируем список
            Mylist = new LinkedList();
            //Инициализируем объект bitmap, копируем размер drawBox в него
            bitmapDraw = new Bitmap(drawBox.Width, drawBox.Height);
            //Инициализация g
            g = Graphics.FromImage(bitmapDraw);
            g.Clear(Color.White);
            drawBox.Image = bitmapDraw;

            model = new Model(drawBox.Width, drawBox.Height, chooseColor);
        }

        public bool controlPressed()
        {
            //Зажат ли control
            return (Control.ModifierKeys & Keys.Control) == Keys.Control;
        }

        public void Draw()
        {
            //Очищает 
            g.Clear(Color.White);
            //Рисуем все круги
            Mylist.front();
            while (!Mylist.eol())
            {
                Mylist.getObject().Draw(bitmapDraw);
                Mylist.next();
            }
            //Копируем изображение из буфера в drawBox
            drawBox.Image = bitmapDraw;
        }

        private void allFalse()
        {
            Mylist.front();
            while (!Mylist.eol())
            {
                Mylist.getObject().Current = false;
                Mylist.next();
            }
        }

        public bool inShape(int xPos, int yPos)
        {
            //Если control не зажат,
            //объекты перестанут быть текущими
            //if (!controlPressed())
            allFalse();

            //проверка, находился ли курсор в круге
            Mylist.back();
            while (!Mylist.eol())
            {
                if (Mylist.getObject().inShape(xPos, yPos))
                    return true;
                else
                    Mylist.prev();
            }

            //Обнуляем значения при создании нового объекта
            allFalse();
            return false;
        }

        private void ShapeCreate(object sender, MouseEventArgs e)
        {
            //если курсор не был в уже созданном круге,
            //создасться новый круг и изображение обновится
            if (e.Button == MouseButtons.Left)
            {
                if (!inShape(e.X, e.Y))
                {
                    if (circleMenu.Checked)
                    {
                        Mylist.push_back(new CCircle(e.X, e.Y));
                    }
                    else if (squareMenu.Checked)
                    {
                        Mylist.push_back(new CRectangle(e.X, e.Y));
                    }
                    Draw();
                }
                else
                {
                    Draw();
                }
            }
        }

        //Событие при нажатии клавиши
        private void Paint_KeyDown(object sender, KeyEventArgs e)
        {
            Mylist.front();
            while (!Mylist.eol())
            {
                if (Mylist.getObject().Current)
                    break;
                else
                    Mylist.next();
            };

            //Проверяет, нажата ли клавиша Delete
            if (e.KeyCode == Keys.Delete)
            {
                //Удаляет все объекты со значение current = true
                Mylist.front();
                while (!Mylist.eol())
                {
                    if (Mylist.getObject().Current)
                    {
                        Mylist.erase(Mylist.getCurrent());
                    }
                    else
                        Mylist.next();
                }
                //Вырисовывает
                Draw();
            }

            if (!Mylist.eol() && Mylist.getObject() != null)
                model.ShapeAct(e.KeyCode, Mylist.getObject());

            Draw();
        }
    }

    class Model
    {
        int width;
        int height;
        ColorDialog colorChoose;
        public Model(int width, int height, ColorDialog color)
        {
            this.width = width;
            this.height = height;
            colorChoose = color;
        }
        public void ShapeAct(Keys key, Shape shape)
        {
            if (key == Keys.C)
            {
                colorChoose.Color = shape.hBrush.BackgroundColor;
                colorChoose.ShowDialog();

                shape.hBrush = new HatchBrush(HatchStyle.Cross,
                Color.PaleVioletRed, colorChoose.Color);

                return;
            }
            int y0 = shape.getCentre().Y - shape.Size;
            int y1 = shape.getCentre().Y + shape.Size;
            int x0 = shape.getCentre().X - shape.Size;
            int x1 = shape.getCentre().X + shape.Size;
            switch (key)
            {
                case Keys.OemMinus:
                    {
                        if (shape.getMinSize <= shape.Size - 5)
                            shape.Resize(-5);
                        break;
                    }
                case Keys.Oemplus:
                    {
                        if (x0 - 5 < 0 || y0 - 5 < 0 || x1 + 5 > width || y1 + 5 > height)
                            return;
                        shape.Resize(5);
                        break;
                    }
                case Keys.Down:
                    {
                        if (y1 + 5 > height)
                        {
                            if (y1 < height)
                                shape.Move(0, height - y1);

                            return;
                        }

                        shape.Move(0, 5);
                        break;
                    }
                case Keys.Up:
                    {
                        if (y0 - 5 < 0)
                        {
                            if (y0 > 0)
                                shape.Move(0, -y0);

                            return;
                        }

                        shape.Move(0, -5);
                        break;
                    }
                case Keys.Left:
                    {
                        if (x0 - 5 < 0)
                        {
                            if (x0 > 0)
                                shape.Move(-x0, 0);

                            return;
                        }

                        shape.Move(-5, 0);
                        break;
                    }
                case Keys.Right:
                    {
                        if (x1 + 5 > width)
                        {
                            if (x1 < width)
                                shape.Move(width - x1, 0);

                            return;
                        }

                        shape.Move(5, 0);
                        break;
                    }
            }
        }
    }

}

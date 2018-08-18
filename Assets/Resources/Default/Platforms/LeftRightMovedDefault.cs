using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultTheme
{
    // крайняя противоположная точка - точка симметрично противоположная крайней точке текущего направления относительно стартовой точки

    // класс - задел под смещение по рандомному вектору
    public class LeftRightMovedDefault : Platform
    {
        public float SpeedMoving;
        
        public Vector3 Forvard; // вдоль какого вектора будет смещение, он же задаёт амплитуду

        public bool ForvardVectorGoingRight;

        protected override void DoStart()
        {
            base.DoStart();

            SpeedMoving = 400;

            Forvard = new Vector3(200, 0, 0);
            ForvardVectorGoingRight = true;
        }

        protected override void DoUpdate()
        {
            base.DoUpdate();

            MoveLeftRight();
        }

        //void OnCollisionEnter(Collision collision)
        void OnCollisionEnter()
        {
            // тут возможно бага, когда у юзера залагало и платформа улетела в крайнюю противоположную точку, а там другая платформа и в какую сторону не двигайся, везде коллайдер

            SwapForvardVectorGoingRight();
            MoveLeftRight();
        }

        void MoveLeftRight()
        {
            Vector3 forvard = ForvardVectorGoingRight ? Forvard : -Forvard;

            Vector3 curVec = forvard.normalized * Time.deltaTime * SpeedMoving;

            transform.position += curVec;

            // если расстояние между стартовой и текущей позицией больше допустимого - меняем направление
            if( (transform.position-StartPosition).magnitude > forvard.magnitude ) {
                Vector3 offset = transform.position - StartPosition - forvard;

                // если у игрока всё зависло и deltatime велико настолько что платформа уехала далеко в противоположную сторону, максимальная её позиция - крайняя противоположная точка
                if ( offset.magnitude > 2*forvard.magnitude )
                    transform.position = - ( StartPosition + forvard );
                else
                    // так как сейчас мы находимся за предельной точкой на расстояние offset, а надо быть перед этой точкой на расстояние offset
                    transform.position -= 2*offset;

                // меняем направление
                SwapForvardVectorGoingRight();
            }
        }

        void SwapForvardVectorGoingRight()
        {
            ForvardVectorGoingRight = !ForvardVectorGoingRight;
        }
    }
}
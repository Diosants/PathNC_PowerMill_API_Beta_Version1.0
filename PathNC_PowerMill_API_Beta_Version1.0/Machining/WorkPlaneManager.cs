using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.Geometry;

namespace P1PlateStandard.Setup
{
    public class WorkPlaneManager
    {
        private readonly PMProject _project;


        public WorkPlaneManager(PMProject project)
        {
            _project = project;
        }

        public void CreateG54()
        {
            var origin =  new Point(0, 0, 0);
            var xAxis = new Vector(1, 0, 0);
            var yAxis =  new Vector(0, 1, 0);
            var zAxis =  new Vector(0, 0, 1);

            var workPlane =  new Workplane(origin, xAxis, yAxis, zAxis);
            var wp = _project.Workplanes.CreateWorkplane( workPlane);

            wp.Name = "G54";
            wp.IsActive = true;
        }

        public void CreateG55()
        {

     
            var origin1 = new Point(0, 0, 0);
            var xAxis1 = new Vector(1, 0, 0);
            var yAxis1 = new Vector(0, -1, 0);
            var zAxis1 = new Vector(0, 0, 1);

            var workPlane1 = new Workplane(origin1, xAxis1, yAxis1, zAxis1);
            var wp1 = _project.Workplanes.CreateWorkplane(workPlane1);
            wp1.Name = "G55";
            wp1.IsActive = true;




            

        }
    }
}


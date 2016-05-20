﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GFS.Models;
using GFS.Models.Policies;

namespace GFS.Controllers.Policies
{
    public class DependantsController : Controller
    {
        private GFSContext db = new GFSContext();

        // GET: Dependants
        public ActionResult Index(string searchString)
        {
            var dependants = from m in db.Dependants
                             select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                dependants = dependants.Where(s => s.policyNo.Contains(searchString));

                Dependant d = db.Dependants.ToList().Find(r => r.policyNo == searchString);

                Session["First Name"] = d.fName;
                Session["Last Name"] = d.lName;
                Session["ID Number"] = d.IdNo;
                Session["Age"] = d.age;
                Session["PolicyNo"] = d.policyNo;

                RedirectToAction("Create", "Deceaseds");
            }
            return View(dependants);

        }
        // GET: Dependants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependant dependant = db.Dependants.Find(id);
            if (dependant == null)
            {
                return HttpNotFound();
            }
            return View(dependant);
        }

        // GET: Dependants/Create
        public ActionResult Create()
        {
            var relList = new List<SelectListItem>();
            var DirQuery = from e in db.Relations select e;
            foreach (var m in DirQuery)
            {
                relList.Add(new SelectListItem { Value = m.relationsh, Text = m.relationsh });
            }
            ViewBag.rlist = relList;
            return View();
        }

        // POST: Dependants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "depNo,coveredby,fName,lName,IdNo,dOb,age,relationship,amount,policyPlan,asBeneficiary,addAnotherDep,policyNo")] Dependant dependant)
        {
            Dependant idno = db.Dependants.ToList().Find(x => x.IdNo == dependant.IdNo);
            if (idno != null)
            {
                Session["responce4"] = "Dependant Already Exists, Check ID Number! Covered under: "+idno.coveredby;
                return RedirectToAction("Create");
            }
            //if (ModelState.IsValid)
            //{
            
                else if (Session["owner"] != null)
                {
                    dependant.coveredby = (Session["owner"]).ToString();
                }
                if (Session["polplan"] != null)
                {
                    dependant.policyPlan = (Session["polplan"]).ToString();
                }
                if (Session["PolicyNo"] != null)
                {
                    dependant.policyNo = (Session["PolicyNo"]).ToString();
                }
                if (dependant.IdNo != null)
                {
                    int year = Convert.ToInt16(dependant.IdNo.Substring(0, 2));
                    int month = Convert.ToInt16(dependant.IdNo.Substring(2, 2));
                    int day = Convert.ToInt16(dependant.IdNo.Substring(4, 2));
                    int gender = Convert.ToInt16(dependant.IdNo.Substring(7, 1));
                    dependant.dOb = Convert.ToDateTime(day + "-" + month + "-" + year);
                }
                if (dependant.dOb != null)
                {
                    dependant.age = (DateTime.Now.Year) - (dependant.dOb.Year);
                }
                if (dependant.policyPlan == "Plan A")
                {
                    if (dependant.age <= 64)
                    {
                        dependant.amount = 80;
                    }
                    else if (dependant.age <= 74)
                    {
                        dependant.amount = 160;
                    }
                    else if (dependant.age <= 84)
                    {
                        dependant.amount = 310;
                    }
                    else
                    {
                        Session["responce4"] = "Cannot add person over 84 years from this plan!";
                        return View("Create");
                    }
                }
                if (dependant.policyPlan == "Plan B")
                {
                    if (dependant.age <= 64)
                    {
                        dependant.amount = 60;
                    }
                    else if (dependant.age <= 74)
                    {
                        dependant.amount = 130;
                    }
                    else if (dependant.age <= 84)
                    {
                        dependant.amount = 170;
                    }
                    else
                    {
                        dependant.amount = 280;
                    }
                }
                if (dependant.policyPlan == "Plan C1")
                {
                    if (dependant.age <= 64)
                    {
                        dependant.amount = 46;
                    }
                    else if (dependant.age <= 74)
                    {
                        dependant.amount = 82;
                    }
                    else if (dependant.age <= 84)
                    {
                        dependant.amount = 109;
                    }
                    else
                    {
                        dependant.amount = 200;
                    }
                }
                if (dependant.policyPlan == "Plan C2")
                {
                    if (dependant.age <= 64)
                    {
                        dependant.amount = 64;
                    }
                    else if (dependant.age <= 74)
                    {
                        dependant.amount = 117;
                    }
                    else if (dependant.age <= 84)
                    {
                        dependant.amount = 158;
                    }
                    else
                    {
                        dependant.amount = 234;
                    }
                }
                if (dependant.policyPlan == "Plan C3")
                {
                    if (dependant.age <= 64)
                    {
                        dependant.amount = 82;
                    }
                    else if (dependant.age <= 74)
                    {
                        dependant.amount = 153;
                    }
                    else if (dependant.age <= 84)
                    {
                        dependant.amount = 207;
                    }
                    else
                    {
                        Session["responce4"] = "Cannot add person over 84 years from this plan!";
                        return View("Create");
                    }
                }
                else if (dependant.age <= 18 || dependant.relationship == "Spouse")
                {
                    dependant.amount = 0;
                }
                db.Dependants.Add(dependant);
                db.SaveChanges();
                if (dependant != null)
                {
                    Session["Dep"] = dependant;
                }
                if (dependant.asBeneficiary == true)
                {
                    Session["finame"] = dependant.fName;
                    Session["laname"] = dependant.lName;
                    Session["Id"] = dependant.IdNo;
                    Session["relation"] = dependant.relationship;
                }
                if (dependant.asBeneficiary == false)
                {
                    Session["finame"] = null;
                    Session["laname"] = null;
                    Session["Id"] = null;
                    Session["relation"] = null;
                }
                if (dependant.addAnotherDep == true)
                {
                    return RedirectToAction("Create", "Dependants");
                }
                else if (dependant.addAnotherDep == false)
                {
                    return RedirectToAction("Create", "Beneficiaries");
                }
            //}
              return RedirectToAction("Create", "Beneficiaries");
        }

        // GET: Dependants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependant dependant = db.Dependants.Find(id);
            if (dependant == null)
            {
                return HttpNotFound();
            }
            return View(dependant);
        }

        // POST: Dependants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "depNo,coveredby,fName,lName,IdNo,dOb,age,relationship,amount,policyPlan,asBeneficiary,addAnotherDep,policyNo")] Dependant dependant)
        {
            if (ModelState.IsValid)
            {
                //dependant.addAnotherDep = false;
                db.Entry(dependant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dependant);
        }

        // GET: Dependants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependant dependant = db.Dependants.Find(id);
            if (dependant == null)
            {
                return HttpNotFound();
            }
            return View(dependant);
        }

        // POST: Dependants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dependant dependant = db.Dependants.Find(id);
            db.Dependants.Remove(dependant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

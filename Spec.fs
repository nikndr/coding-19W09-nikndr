namespace OOAD19.Coding.W09

open Client
open Domain
open FsCheck.Xunit
open FsCheck


module Spec =

    let vipcust() =
        { id = 777
          name = "Jason Statham"
          street = "7th Avenue"
          city = "New York"
          state = "NY"
          zip = "10011" }

    [<Property>]
    let ``Basic order calculation``() =
        Gen.choose (5, 20)
        |> Arb.fromGen
        |> Prop.forAll
        <| fun qty ->
            let price = qty * 9 |> decimal
            let pid = addProduct "pro1" price false

            let vipcust = vipcust()
            let total =
                (vipcust,
                 [ { productId = pid
                     qty = qty } ])
                ||> newOrder
                |> getTotal

            let doubleTotal =
                (vipcust,
                 [ { productId = pid
                     qty = qty * 2 } ])
                ||> newOrder
                |> getTotal

            total.total * 2m = doubleTotal.total && total.taxTotal = 0m && total.total > 0m

    [<Property>]
    let ``Updating order``() =
        Gen.choose (11, 33)
        |> Arb.fromGen
        |> Prop.forAll
        <| fun qty ->
        
            let vipcust = vipcust()

            let price = qty * 7 |> decimal
            let pid1 = addProduct "pro1" price false
            let pid2 = addProduct "pro2" price false

            let total1and2 =
                (vipcust,
                 [ { productId = pid1
                     qty = qty }
                   { productId = pid2
                     qty = qty } ])
                ||> newOrder
                |> getTotal

            let uoid =
                (vipcust,
                 [ { productId = pid1
                     qty = qty } ])
                ||> newOrder

            let total1 = uoid |> getTotal

            uoid
            |> updateOrder
            <| [ { productId = pid1
                   qty = qty }
                 { productId = pid2
                   qty = qty } ]

            let totalU1and2 = uoid |> getTotal

            uoid
            |> updateOrder
            <| [ { productId = pid2
                   qty = qty } ]

            let total2 = uoid |> getTotal


            total1and2.total = totalU1and2.total && total1.total + total2.total = totalU1and2.total
            && totalU1and2.total > 0m

    [<Property>]
    let ``Updating product price affects draft order``() =
        Gen.choose (11, 33)
        |> Arb.fromGen
        |> Prop.forAll
        <| fun qty ->

            let vipcust = vipcust()

            let price = qty * 7 |> decimal
            let pid = addProduct "pro1" price false

            let oid =
                (vipcust,
                 [ { productId = pid
                     qty = qty } ])
                ||> newOrder

            let total = oid |> getTotal

            updateProduct
                { id = pid
                  name = "pro1"
                  price = price * 2m
                  taxable = false }

            let totalu = oid |> getTotal

            let status = oid |> getStatus
            
            total.total * 2m = totalu.total 
            && total.total > 0m
            && status = Draft

    [<Property>]
    let ``Updating product price does not affect submitted and paid orders``() =
        Gen.choose (9, 41)
        |> Arb.fromGen
        |> Prop.forAll
        <| fun qty ->
            
            let vipcust = vipcust()

            let price = qty * 4 |> decimal
            let pid = addProduct "pro1" price false

            let oid =
                (vipcust,
                 [ { productId = pid
                     qty = qty } ])
                ||> newOrder

            oid |> submitOrder

            let status1 = oid |> getStatus

            let total = oid |> getTotal

            updateProduct
                { id = pid
                  name = "pro1"
                  price = price * 2m
                  taxable = false }

            let totalu1 = oid |> getTotal

            oid |> payOrder

            let status2 = oid |> getStatus

            updateProduct
                { id = pid
                  name = "pro1"
                  price = price * 3m
                  taxable = false }

            let totalu2 = oid |> getTotal

            total.total = totalu1.total && total.total = totalu2.total && total.total > 0m && status1 = Submitted
            && status2 = Paid

    [<Property>]
    let ``Taxable order calculation``() =
        Gen.choose (13, 42)
        |> Arb.fromGen
        |> Prop.forAll
        <| fun qty ->

            let vipcust = vipcust()

            let price = qty * 4 |> decimal
            let pidnt = addProduct "pro1" price false
            let pidt = addProduct "pro2" price true

            let totalt =
                (vipcust,
                 [ { productId = pidt
                     qty = qty } ])
                ||> newOrder
                |> getTotal

            let totalnt =
                (vipcust,
                 [ { productId = pidnt
                     qty = qty } ])
                ||> newOrder
                |> getTotal

            let totaltnt =
                (vipcust,
                 [ { productId = pidt
                     qty = qty }
                   { productId = pidnt
                     qty = qty } ])
                ||> newOrder
                |> getTotal

            totalt.subtotal + totalt.taxTotal = totalt.total && totalt.total + totalnt.total = totaltnt.total
            && totalt.taxTotal > 0m && totaltnt.taxTotal > 0m
 
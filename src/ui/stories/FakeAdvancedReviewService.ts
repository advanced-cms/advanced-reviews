import screenshots from "../screenshots/screenshots.json";

const createComment = (author: string, text: string, date: Date, screenshot: string = null) => {
    return {
        author,
        text,
        date,
        screenshot
    }
};

const reviewLocations = [
    {
        id: "1",
        data: {
            positionX: 10,
            positionY: 80,
            propertyName: "Page name",
            isDone: false,
            firstComment: createComment("Alfred", "Rephrase it. ", new Date("2019-01-01")),
            comments: [
                createComment("Lina", "Could you describe it better?", new Date("2019-01-02"), screenshots.idylla),
                createComment("Alfred", "Remove last sentence and include more information in first paragraph.", new Date("2019-01-03")),
                createComment("Lina", "Ok, done.", new Date("2019-01-04"), screenshots.idylla),
                createComment("Alfred", "I still see old text", new Date("2019-03-18"), screenshots.idylla),
                createComment("Lina", "Probably something with the CMS. Now it should be ok", new Date("2019-03-19")),
                createComment("Alfred", "Looks ok.", new Date("2019-03-19")),
                createComment("Lina", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean sed nisi in erat posuere luctus.", new Date("2019-03-20")),
                createComment("Alfred", "Vivamus sem est, aliquet eget nunc quis, imperdiet cursus sapien. Mauris ullamcorper dui ut nisl vulputate vestibulum.", new Date("2019-03-21")),
                createComment("Lina", "Sed non nisi in odio facilisis aliquam eget volutpat augue. Phasellus vitae auctor risus, non luctus dolor.", new Date("2019-03-22")),
                createComment("Alfred", "Integer sed libero at odio mattis sodales. Ut dapibus erat cursus porttitor malesuada.", new Date("2019-03-23")),
            ]
        }
    },
    {
        id: "2",
        data: {
            positionX: 100,
            positionY: 150,
            propertyName: "Page body",
            isDone: false,
            firstComment: createComment("John", "Remove the above text. It's already included in another article.", new Date("2019-01-01")),
            comments: [
                createComment("Lina", "Etiam viverra ante mauris, eget pretium quam ultrices vel.", new Date("2019-01-02")),
                createComment("Alfred", "Maecenas non lorem et lectus ultrices consequat vel eget magna.", new Date("2019-01-03")),
                createComment("Lina", "Aenean malesuada nibh a ante scelerisque consequat.", new Date("2019-01-04")),
                createComment("Alfred", "Phasellus eu nulla ac tellus semper imperdiet nec eu nulla.", new Date("2019-03-18")),
                createComment("Lina", "Etiam vel tortor gravida, venenatis enim at, finibus dolor.", new Date("2019-03-19")),
                createComment("Alfred", "Nunc ultricies tortor semper leo efficitur, vitae viverra ligula semper.", new Date("2019-03-19")),
                createComment("Lina", "Nunc ultricies tortor semper leo efficitur, vitae viverra ligula semper.", new Date("2019-03-20")),
                createComment("Alfred", "Ut viverra odio ligula, vitae gravida arcu aliquam id.", new Date("2019-03-21")),
                createComment("Lina", "Pellentesque elementum sem quis eleifend gravida.", new Date("2019-03-22")),
                createComment("Alfred", "Quisque tincidunt mi a pretium rutrum.", new Date("2019-03-23")),
            ]
        }
    },
    {
        id: "3",
        data: {
            positionX: 250,
            positionY: 200,
            propertyName: "Main ContentArea",
            isDone: false
        }
    },
    {
        id: "4",
        data: {
            positionX: 125,
            positionY: 330,
            propertyName: "Description",
            isDone: false
        }
    }
];

export default class FakeAdvancedReviewService implements AdvancedReviewService {
    add(id: string, data: any): Promise<any> {
        let result: any = new Promise(resolve => {
            resolve();
        });
        result.otherwise = () => {};
        result.then = (callback) => {
            let reviewLocation = reviewLocations.find(x => x.id === id)
            if (reviewLocation) {
                reviewLocation.data = data;
            } else {
                reviewLocation = {
                    id: new Date().getTime().toString(),
                    data: data
                }
            }
            reviewLocations.push(reviewLocation);

            callback(reviewLocation);
            return result;
        };
        return result;
    }

    load(): Promise<any[]> {
        const convertedResult = reviewLocations.map(x => {
            return {
                id: x.id,
                data: JSON.stringify(x.data)
            }
        });

        return new Promise(resolve => {
            resolve(convertedResult);
        });
    }
}
